using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FileTransferContract;
using FileTransferContract.Class;

namespace FileTransferService
{
  
    public interface IRuleAction
    {
        void ReplaceTransferRule(TransferRule tr);
        void Start();
        void Stop();
    }

   






    public interface IRuleManager: IDisposable
    {
        void AddRule(TransferRule rule);
        void RemoveRule(string[] GUID);
        void ReplaceRule(string GUID, TransferRule rule);
        List<ARule> GetRule();
        void StartRule(string[] GUID);
        void StopRule(string[] GUID);
    }

    public class RuleManager : IRuleManager
    {
        private IRepository repository;
        private List<RuleAction> RuleActions = new List<RuleAction>();
        private ILogger logger;
        public RuleManager(IRepository repository, ILogger logger)
        {
            this.repository = repository;
            this.logger = logger;
            LoadData();
        }

        private void LoadData()
        {
            RuleActions.Clear();
            var list = repository.Load();
            foreach (var item in list)
            {
                RuleActions.Add(new RuleAction(item, logger));
            }
        }

        private void SaveData()
        {
            repository.Save(RuleActions.Select(x=>x.TransferRule));
        }
        public void AddRule(TransferRule rule)
        {
            RuleActions.Add(new RuleAction(rule, logger));
        }

        public void RemoveRule(string[] GUID)
        {
            foreach (var guid in GUID)
            {
                var item = RuleActions.FirstOrDefault(x => x.GUID == guid);
                if(item!=null)
                    RuleActions.Remove(item);
            }
          
        }

        public void ReplaceRule(string GUID, TransferRule rule)
        {
            var item = RuleActions.FirstOrDefault(x => x.GUID == GUID);
            item?.ReplaceTransferRule(rule);
        }

        public List<ARule> GetRule()
        {
            return RuleActions.Select(x=>(ARule) x).ToList();
        }

        public void StartRule(string[] GUID)
        {
            foreach (var guid in GUID)
            {
                var item = RuleActions.FirstOrDefault(x => x.GUID == guid);
                item?.Start();
            }
        }

        public void StopRule(string[] GUID)
        {
            foreach (var guid in GUID)
            {
                var item = RuleActions.FirstOrDefault(x => x.GUID == guid);
                item?.Stop();
            }
        }

        public void Dispose()
        {
            foreach (var item in RuleActions)
            {
                item.Stop();
            }
        }
    }


    public class RuleAction : ARule, IRuleAction
    {
        public override bool isActive => task?.Status == TaskStatus.Running || task?.Status == TaskStatus.WaitingForActivation || task?.Status == TaskStatus.WaitingForChildrenToComplete;
        public override TransferRule TransferRule { get; set; }

        public override string GUID { get; set; } = Guid.NewGuid().ToString();
        private ILogger logger;
        public RuleAction(TransferRule tr, ILogger logger)
        {
            TransferRule = tr;
            this.logger = logger;
        }


        public void ReplaceTransferRule(TransferRule tr)
        {
            this.Stop();
            TransferRule = tr;
        }



        CancellationTokenSource cts;
        private Task task;
        public void Start()
        {
            logger?.Add($"Запуск задачи переноса из {TransferRule.PathSource} в {TransferRule.PathDestination} от имени пользователя {TransferRule.User.FULL_USER}", false);
            cts = new CancellationTokenSource();
            task = Task.Run(async () =>
            {
                try
                {
                    while (!cts.IsCancellationRequested)
                    {
                        FileMover.MoveFiles(TransferRule, logger);
                        await Task.Delay(TransferRule.TimeOut * 1000);
                    }
                    logger?.Add($"Остановка задачи переноса из {TransferRule.PathSource} в {TransferRule.PathDestination} от имени пользователя {TransferRule.User.FULL_USER}", false);
                }
                catch (Exception e)
                {
                    logger?.Add($"Ошибка: {e.Message}{Environment.NewLine}{e.StackTrace}", true);
                }

            }, cts.Token);
        }

        public void Stop()
        {
            cts?.Cancel();
        }
    }


}
