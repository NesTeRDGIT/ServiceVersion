﻿using ClientServiceWPF.Class;
using ClientServiceWPF.MEK_RESULT.ACTMEK;
using ServiceLoaderMedpomData.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using MessageBox = System.Windows.MessageBox;
using LogType = ClientServiceWPF.Class.LogType;
using System.Collections.ObjectModel;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.IO;
using ExcelManager;

namespace ClientServiceWPF.ExportSchetFactureFile
{
    internal class ExportShetFactureFileViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public ExportShetFactureFileViewModel()
        {
            CurrentDate = DateTime.Now;
        }
        public ExportShetFactureFileViewModel(Dispatcher dispatcher)
        {
            CurrentDate = DateTime.Now;
            this.dispatcher = dispatcher;
        }
        public ObservableCollection<LogItem> Logs { get; set; } = new ObservableCollection<LogItem>();
        public ProgressItem Progress1 { get; } = new ProgressItem();
        public DateTime CurrentDate
        {
            get
            {
                return _currentDate;
            }
            set
            {
                _currentDate = value;
                RaisePropertyChanged();
            }
        }
        public bool IsOperationRun
        {
            get => _IsOperationRun;
            set
            {
                _IsOperationRun = value;
                RaisePropertyChanged();
                //CommandManager.InvalidateRequerySuggested();
            }
        }
        public ICommand ExportSchetFactureFileComand => new Command(async o =>
        {
            try
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    IsOperationRun = true;
                    Logs.Clear();
                    Progress1.IsIndeterminate = true;
                    cts = new CancellationTokenSource();
                    //var files = await GetFileAsync(fbd.SelectedPath, CurrentDate, cts.Token); 

                    var tasks = new List<Task>
                    {
                        Task.Run(() =>
                        {
                            GetFile(fbd.SelectedPath, CurrentDate, cts.Token);
                        }
                    )
                    };
                    await Task.WhenAll(tasks);
                    if (MessageBox.Show(@"Завершено. Показать файл?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        ShowSelectedInExplorer.FilesOrFolders(fbd.SelectedPath);
                    }

                    Progress1.IsIndeterminate = false;
                    Progress1.Text = "";
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.FullMessage());
            }
            finally
            {
                Progress1.Clear("");
                IsOperationRun = false;
            }
        }, o => !IsOperationRun);
        public ICommand BreakCommand => new Command(o =>
        {
            try
            {
                cts?.Cancel();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }, o => IsOperationRun);

        private async void GetFile(string selectedPath, DateTime currentDate, CancellationToken token)
        {
            try
            {
                AddLogs(LogType.Info, "Формирование файлов");
                var dtSMO = new DataTable();
                using (var conn = new OracleConnection(AppConfig.Property.ConnectionString))
                {
                    using (var oda = new OracleDataAdapter($"select * from nsi.f002 t where t.tf_okato = 76000 and t.d_end is null", conn))
                    {
                        dispatcher.Invoke(() => { Progress1.Text = "Запрос данных f002"; });
                        oda.Fill(dtSMO);
                    }
                }

                foreach (DataRow row in dtSMO.Rows)
                {
                    var smoCode = row["smocod"].ToString();
                    var smoName = row["nam_smok"].ToString();

                    AddLogs(LogType.Info, $"Формирование случаев CMO {smoCode}");

                    var path = Path.Combine(fbd.SelectedPath, smoCode);
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    var tasks = new List<Task>();

                    tasks.Add(Task.Run(() =>
                    {
                        AddLogs(LogType.Info, $"Формирование итоговых реестров");
                        ItogReestrXls(path, $"{smoCode}-{smoName}", CurrentDate, cts.Token);
                    }));

                    tasks.Add(Task.Run(() =>
                    {
                        AddLogs(LogType.Info, $"Формирование счетов фактур");
                        SchetFactureXls(path, $"{smoCode}-{smoName}", CurrentDate, cts.Token);
                    }));

                    await Task.WhenAll(tasks);
                }

                AddLogs(LogType.Info, $"Формирование файлов завершено");
            }
            catch (Exception ex)
            {
                AddLogs(LogType.Error, ex.FullMessage());
                MessageBox.Show(ex.Message);
            }
        }

        private void SchetFactureXls(string path, string smoData, DateTime currentDate, CancellationToken token)
        {
            var smoCode = smoData.Split('-')[0];
            var smoName = smoData.Split('-')[1];
            var dtFakture = new DataTable();
            using (var conn = new OracleConnection(AppConfig.Property.ConnectionString))
            {
                using (var oda = new OracleDataAdapter($"select * from FAKTURA_2021 t where smo = {smoCode}", conn))
                {
                    dispatcher.Invoke(() => { Progress1.Text = "Запрос данных FAKTURA_2021"; });
                    oda.Fill(dtFakture);
                }
            }

        }

        private void ItogReestrXls(string path, string smoData, DateTime currentDate, CancellationToken token)
        {
            var smoCode = smoData.Split('-')[0];
            var smoName = smoData.Split('-')[1];

            var dtReestr = new DataTable();
            using (var conn = new OracleConnection(AppConfig.Property.ConnectionString))
            {
                using (var oda = new OracleDataAdapter($"select * from v_lpu_sum_2021 t where smo = {smoCode}", conn))
                {
                    dispatcher.Invoke(() => { Progress1.Text = "Запрос данных v_lpu_sum_2021"; });
                    oda.Fill(dtReestr);
                }
            }


            foreach (DataRow row in dtReestr.Rows)
            {
                try
                {
                    var moCode = row["code_mo"];
                    var fileName = $@"{path}\{smoCode} Итоговый реестр {moCode} за {currentDate.ToString("MMMM yyyy")}.xlsx";

                    moCode = "750001";
                    var dtMo = new DataTable();
                    using (var conn = new OracleConnection(AppConfig.Property.ConnectionString))
                    {
                        using (var oda = new OracleDataAdapter($"select * from nsi.f003 t where mcod = '{moCode}'", conn))
                        {
                            dispatcher.Invoke(() => { Progress1.Text = "Запрос данных nsi.f003"; });
                            oda.Fill(dtMo);
                        }
                    }
                    
                    int rIndexDtMo = dtMo.Rows.IndexOf(dtMo.Select($" mcod = {moCode}")[0]);
                    string moName = Convert.ToString(dtMo.Rows[rIndexDtMo]["nam_mok"]);
                    
                    File.Copy(ITOG_REESTR_TEMPLATE, fileName, true);
                    using (var efm = new ExcelOpenXML())
                    {
                        efm.OpenFile(fileName, 0);
                        
                        uint rowIndex = 3;
                        efm.PrintCell(rowIndex, 1, $"медицинских услуг, оказанных {moName}", null);
                        rowIndex += 2;
                        efm.PrintCell(rowIndex, 1, $"гражданам, застрахованным в  {smoName}", null);
                        rowIndex += 2; 
                        efm.PrintCell(rowIndex, 1, $"Согласно прилагаемым реестрам, за {currentDate.ToString("MMMM")} оказаны медицинские услуги на сумму:", null);
                        rowIndex += 2;

                        efm.PrintCell(rowIndex, 2, row["amb_sum"].ToString(), null);
                        rowIndex++;
                        efm.PrintCell(rowIndex, 2, row["amb_mbt_sum"].ToString(), null);
                        rowIndex++;
                        efm.PrintCell(rowIndex, 2, row["fap_sum"].ToString(), null);
                        rowIndex++;
                        efm.PrintCell(rowIndex, 2, row["smp_sum"].ToString(), null);
                        rowIndex++; 
                        efm.PrintCell(rowIndex, 2, row["tromb"].ToString(), null);
                        rowIndex += 2;

                        efm.PrintCell(rowIndex, 2, row["pol"].ToString(), null);
                        rowIndex++;
                        efm.PrintCell(rowIndex, 2, row["dializ_a"].ToString(), null);
                        rowIndex++;
                        efm.PrintCell(rowIndex, 2, row["rnk"].ToString(), null);
                        rowIndex++;
                        efm.PrintCell(rowIndex, 2, row["dli"].ToString(), null);
                        rowIndex++;
                        efm.PrintCell(rowIndex, 2, row["neot"].ToString(), null);
                        rowIndex += 2;
                        
                        efm.PrintCell(rowIndex, 2, row["stac"].ToString(), null);
                        rowIndex++;
                        efm.PrintCell(rowIndex, 2, row["dializ_c"].ToString(), null);
                        rowIndex += 2;
                        
                        efm.PrintCell(rowIndex, 2, row["ds"].ToString(), null);
                        rowIndex++;
                        efm.PrintCell(rowIndex, 2, row["eko"].ToString(), null);
                        rowIndex += 2;  
                        
                        efm.PrintCell(rowIndex, 2, row["hmp"].ToString(), null);
                        rowIndex += 2;

                        efm.PrintCell(rowIndex, 2, row["disp"].ToString(), null);
                        rowIndex++; 
                        efm.PrintCell(rowIndex, 2, row["ud"].ToString(), null);
                        rowIndex++;
                        efm.PrintCell(rowIndex, 2, row["other"].ToString(), null);
                        rowIndex += 2;

                        efm.PrintCell(rowIndex, 2, row["s_all"].ToString(), null);

                        efm.MarkAsFinal(true);
                        efm.Save();
                    }
                }
                catch (Exception ex)
                {
                    AddLogs(LogType.Error, ex.FullMessage());
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void AddLogs(LogType type, params string[] Message)
        {
            dispatcher.Invoke(() =>
            {
                foreach (var mes in Message)
                {
                    Logs.Add(new LogItem(type, mes));
                }
            }
            );
        }

        private static string LocalFolder => AppDomain.CurrentDomain.BaseDirectory;
        private string ITOG_REESTR_TEMPLATE { get; set; } = Path.Combine(LocalFolder, "TEMPLATE", "TEMPLATE_ITOG_REESTR.xlsx");
        private string ITOG_REESTR_MBT__TEMPLATE { get; set; } = Path.Combine(LocalFolder, "TEMPLATE", "TEMPLATE_ITOG_REESTR_MBT.xlsx");
        private readonly Dispatcher dispatcher;
        private CancellationTokenSource cts;
        private FolderBrowserDialog fbd = new FolderBrowserDialog();
        private string connStr = AppConfig.Property.ConnectionString;
        private DateTime _currentDate;
        private bool _IsOperationRun;
    }
}
