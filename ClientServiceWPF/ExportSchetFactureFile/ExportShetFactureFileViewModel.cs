using ClientServiceWPF.Class;
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
        public ExportShetFactureFileViewModel()
        {
            CurrentDate = DateTime.Now;
            Task.Run(() => { InitF002(); }); // Smo
            Task.Run(() => { InitF003(); }); // Mo
            Task.Run(() => { InitVolumRubric(); }); // VolumRubric
        }
        public ExportShetFactureFileViewModel(Dispatcher dispatcher)
        {
            CurrentDate = DateTime.Now;
            this.dispatcher = dispatcher;
            Task.Run(() => { InitF002(); });
            Task.Run(() => { InitF003(); });
            Task.Run(() => { InitVolumRubric(); });
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

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

                    await Task.Run(() =>
                    {
                        GetFile(fbd.SelectedPath, CurrentDate, cts.Token);
                    });

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

        private void GetFile(string selectedPath, DateTime currentDate, CancellationToken token)
        {
            try
            {
                AddLogs(LogType.Info, "Формирование файлов");

                foreach (DataRow row in _dtF002.Rows)
                {
                    var smoCode = row["smocod"].ToString();
                    var smoName = row["nam_smok"].ToString();

                    AddLogs(LogType.Info, $"Формирование случаев CMO {smoCode}");

                    var path = Path.Combine(fbd.SelectedPath, smoCode);
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);


                    AddLogs(LogType.Info, $"Формирование итоговых реестров");
                    ItogReestrXls(path, $"{smoCode}-{smoName}", CurrentDate, cts.Token);

                    AddLogs(LogType.Info, $"Формирование счетов фактур");
                    SchetFactureXls(path, $"{smoCode}-{smoName}", CurrentDate, cts.Token);
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
            var dtMurFin = new DataTable();
            using (var conn = new OracleConnection(connStr))
            {
                var command = $"select * " +
                    $"from FAKTURA_2021 t" +
                    $"  where smo = {smoCode} and year = {currentDate.Year} and month = {currentDate.Month}";
                using (var oda = new OracleDataAdapter(command, conn))
                {
                    dispatcher.Invoke(() => { Progress1.Text = "Запрос данных FAKTURA_2021"; });
                    oda.Fill(dtFakture);
                }

                command = $"select * from nsi.mur_fin t" +
                          $" where smo = {smoCode} and year = {currentDate.Year} and month = {currentDate.Month}";
                using (var oda = new OracleDataAdapter(command, conn))
                {
                    dispatcher.Invoke(() => { Progress1.Text = "Запрос данных nsi.MUR_FIN"; });
                    oda.Fill(dtMurFin);
                }
            }


            foreach (DataRow mo in _dtF003.Rows)
            {

                var moCode = mo["mcod"].ToString();
                var moName = GetNameMoByCodeMo(moCode);
                decimal amountByCompany = 0;
                var fileName = $@"{path}\{moCode} Счет-фактура за {currentDate:MMMM.yyyy}.xlsx";
                bool isFirstPassFile = true;
                File.Copy(SCHET_FACTURE_TEMPLATE, fileName, true);
                using (var efm = new ExcelOpenXML())
                {
                    try
                    {
                        efm.OpenFile(fileName, 0);

                        var StyleLeftText = efm.CreateType(new FontOpenXML()
                        {
                            size = 11,
                            fontname = "Times New Roman",
                            HorizontalAlignment = HorizontalAlignmentV.Left,
                            wordwrap = true
                        }, new BorderOpenXML(), null);
                        var StyleCenterText = efm.CreateType(new FontOpenXML()
                        {
                            size = 11,
                            fontname = "Times New Roman",
                            HorizontalAlignment = HorizontalAlignmentV.Center,
                            wordwrap = true,
                        }, new BorderOpenXML(), null);
                        var StyleLeftTextBorderNone = efm.CreateType(new FontOpenXML()
                        {
                            size = 11,
                            fontname = "Times New Roman",
                            HorizontalAlignment = HorizontalAlignmentV.Left,
                            wordwrap = true,
                        }, null, null); ;
                        var StyleCenterBoldText = efm.CreateType(new FontOpenXML()
                        {
                            size = 11,
                            fontname = "Times New Roman",
                            HorizontalAlignment = HorizontalAlignmentV.Center,
                            Bold = true
                        }, new BorderOpenXML(), null);
                        var StyleCenterBoldNumeric = efm.CreateType(new FontOpenXML()
                        {
                            size = 11,
                            fontname = "Times New Roman",
                            HorizontalAlignment = HorizontalAlignmentV.Center,
                            Bold = true,
                            Format = (uint)DefaultNumFormat.F4
                        }, new BorderOpenXML(), null);
                        var StyleCenterNumeric = efm.CreateType(new FontOpenXML()
                        {
                            size = 11,
                            fontname = "Times New Roman",
                            HorizontalAlignment = HorizontalAlignmentV.Center,
                            Format = (uint)DefaultNumFormat.F4
                        }, new BorderOpenXML(), null);                         
                        var StyleCenterNumericBorderNone = efm.CreateType(new FontOpenXML()
                        {
                            size = 11,
                            fontname = "Times New Roman",
                            HorizontalAlignment = HorizontalAlignmentV.Center,
                            Format = (uint)DefaultNumFormat.F4
                        }, null, null);

                        uint rowIndex = 1;
                        efm.PrintCell(rowIndex, 1, $"{moName.ToUpper()}", null);
                        rowIndex = 3;
                        efm.PrintCell(rowIndex, 1, $"за оказанные медицинские услуги по территориальной программе ОМС пациентам застрахованным в {smoName}", null);

                        rowIndex = 4;
                        efm.PrintCell(rowIndex, 1, $"{currentDate:MMMM}", null);

                        rowIndex = 5;
                        efm.PrintCell(rowIndex, 2, $"{smoName}", null);

                        foreach (var fond in _listExistingFond)
                        {
                            bool isFirstPassFond = true;
                            foreach (DataRow volumRubric in _dtVolumRubric.Rows)
                            {
                                bool isFirstPassRubric = true;

                                var listVolumRubric = dtFakture.Select($"code_mo = '{moCode}' " +
                                    $"and tip = '{volumRubric["volum_rubric_id"]}'" +
                                    $"and fond = {fond}");
                                if (listVolumRubric.Length == 0)
                                    continue;

                                decimal sumSluch = 0;
                                decimal sumKU = 0;
                                decimal sumSAll = 0;
                                switch (fond)
                                {
                                    case 1:
                                        if (isFirstPassFile)
                                        {
                                            var fin_s_amb = "";
                                            var fin_s_smp = "";
                                            var fin_s_fap = "";

                                            var murFin = dtMurFin.Select($"code_mo = {moCode} and " +
                                                $"smo = {smoCode} and year = {currentDate.Year} " +
                                                $"and month = {currentDate.Month}").FirstOrDefault();

                                            if (murFin is null)
                                            {
                                                isFirstPassFile = false;
                                                fin_s_amb = "0, 00";
                                                fin_s_smp = "0, 00";
                                                fin_s_fap = "0, 00";
                                            }
                                            else
                                            {
                                                fin_s_amb = murFin["fin_s_amb"].ToString();
                                                fin_s_smp = murFin["fin_s_smp"].ToString();
                                                fin_s_fap = murFin["fin_s_fap"].ToString();
                                                amountByCompany += Round(fin_s_amb);
                                                amountByCompany += Round(fin_s_smp);
                                                amountByCompany += Round(fin_s_fap);
                                            }

                                            rowIndex = 7;
                                            efm.PrintCell(rowIndex, 1, $"Оплата услуг в рамках подушевого финансирования на сумму:", StyleCenterBoldText);
                                            efm.PrintCell(rowIndex, 5, Round(fin_s_amb), StyleCenterBoldNumeric);
                                            efm.AddMergedRegion(new CellRangeAddress(rowIndex, 1, rowIndex, 2));

                                            rowIndex = 8;
                                            efm.PrintCell(rowIndex, 1, $"Оплата скорой медицинской помощи в рамках подушевого финансирования на сумму:", StyleCenterBoldText);
                                            efm.PrintCell(rowIndex, 5, Round(fin_s_smp), StyleCenterBoldNumeric);
                                            efm.AddMergedRegion(new CellRangeAddress(rowIndex, 1, rowIndex, 2));

                                            rowIndex = 9;
                                            efm.PrintCell(rowIndex, 1, $"Оплата услуг в рамках финансирования ФАП:", StyleCenterBoldText);
                                            efm.PrintCell(rowIndex, 5, Round(fin_s_fap), StyleCenterBoldNumeric);
                                            efm.AddMergedRegion(new CellRangeAddress(rowIndex, 1, rowIndex, 2));

                                            rowIndex = 11;
                                            efm.PrintCell(rowIndex, 1, $"в том числе:", null);
                                        }

                                        if (isFirstPassFond)
                                        {
                                            rowIndex += 2;
                                            efm.PrintCell(rowIndex, 1, $"Код услуги", StyleCenterText);
                                            efm.PrintCell(rowIndex, 2, $"Наименование услуги", StyleCenterText);
                                            efm.PrintCell(rowIndex, 3, $"Кол-во случаев", StyleCenterText);
                                            efm.PrintCell(rowIndex, 4, $"Кол-во услуг (к/дней, п/дней, посещений, УЕТ)", StyleCenterText);
                                            rowIndex++;
                                            isFirstPassFond = false;
                                        }

                                        sumSluch = 0;
                                        sumKU = 0;

                                        if (isFirstPassRubric)
                                        {
                                            efm.PrintCell(rowIndex, 1, $"{volumRubric["name"]}", StyleCenterBoldText);
                                            efm.AddMergedRegion(new CellRangeAddress(rowIndex, 1, rowIndex, 4));
                                            rowIndex++;
                                            isFirstPassRubric = false;
                                        }

                                        foreach (var item in listVolumRubric)
                                        {
                                            efm.PrintCell(rowIndex, 1, $"{item["cod"]}", StyleLeftText);
                                            efm.PrintCell(rowIndex, 2, $"{item["name_tarif"]}", StyleLeftText);
                                            efm.PrintCell(rowIndex, 3, Round(item["sluch"].ToString()), StyleCenterNumeric);
                                            efm.PrintCell(rowIndex, 4, Round(item["k_u"].ToString()), StyleCenterNumeric);

                                            sumSluch += Round(item["sluch"].ToString());
                                            sumKU += Round(item["k_u"].ToString());
                                            rowIndex++;
                                        }

                                        rowIndex++;
                                        efm.PrintCell(rowIndex, 1, $"Всего по группе:", StyleCenterBoldText);
                                        efm.AddMergedRegion(new CellRangeAddress(rowIndex, 1, rowIndex, 2));
                                        efm.PrintCell(rowIndex, 3, Round(sumSluch.ToString()), StyleCenterBoldNumeric);
                                        efm.PrintCell(rowIndex, 4, Round(sumKU.ToString()), StyleCenterBoldNumeric);

                                        rowIndex++;
                                        rowIndex++;
                                        isFirstPassFile = false;
                                        break;
                                    case 2:
                                        if (isFirstPassFile)
                                        {
                                            if (rowIndex == 5)
                                                rowIndex = 7;
                                        }

                                        if (isFirstPassFond)
                                        {
                                            rowIndex += 2;
                                            efm.PrintCell(rowIndex, 1, $"Оплата услуг вне рамок подушевого финансирования:", StyleCenterBoldText);
                                            efm.AddMergedRegion(new CellRangeAddress(rowIndex, 1, rowIndex, 7));
                                            rowIndex++;
                                            rowIndex++;
                                            efm.PrintCell(rowIndex, 1, $"Код услуги", StyleCenterText);
                                            efm.PrintCell(rowIndex, 2, $"Наименование услуги", StyleCenterText);
                                            efm.PrintCell(rowIndex, 3, $"Кол-во случаев", StyleCenterText);
                                            efm.PrintCell(rowIndex, 4, $"Кол-во услуг (к/дней, п/дней, посещений, УЕТ)", StyleCenterText);
                                            efm.PrintCell(rowIndex, 5, $"Тариф", StyleCenterText);
                                            efm.PrintCell(rowIndex, 6, $"Поправочный коэффициент", StyleCenterText);
                                            efm.PrintCell(rowIndex, 7, $"Стоимость", StyleCenterText);
                                            rowIndex++;
                                            isFirstPassFond = false;
                                        }

                                        sumSluch = 0;
                                        sumKU = 0;
                                        sumSAll = 0;

                                        if (isFirstPassRubric)
                                        {
                                            efm.PrintCell(rowIndex, 1, $"{volumRubric["name"]}", StyleCenterText);
                                            efm.AddMergedRegion(new CellRangeAddress(rowIndex, 1, rowIndex, 7));
                                            rowIndex++;
                                            isFirstPassRubric = false;
                                        }

                                        foreach (var item in listVolumRubric)
                                        {
                                            efm.PrintCell(rowIndex, 1, $"{item["cod"]}", StyleLeftText);
                                            efm.PrintCell(rowIndex, 2, $"{item["name_tarif"]}", StyleLeftText);
                                            efm.PrintCell(rowIndex, 3, Round(item["sluch"].ToString()), StyleCenterNumeric);
                                            efm.PrintCell(rowIndex, 4, Round(item["k_u"].ToString()), StyleCenterNumeric);
                                            efm.PrintCell(rowIndex, 5, Round(item["summa"].ToString()), StyleCenterNumeric);
                                            efm.PrintCell(rowIndex, 6, Round(item["sk"].ToString()), StyleCenterNumeric);
                                            efm.PrintCell(rowIndex, 7, Round(item["s_all"].ToString()), StyleCenterNumeric);

                                            sumSluch += Round(item["sluch"].ToString());
                                            sumKU += Round(item["k_u"].ToString());
                                            sumSAll += Round(item["s_all"].ToString());

                                            rowIndex++;
                                        }

                                        rowIndex++;
                                        efm.PrintCell(rowIndex, 1, $"Всего по группе:", StyleCenterBoldText);
                                        efm.AddMergedRegion(new CellRangeAddress(rowIndex, 1, rowIndex, 2));
                                        efm.PrintCell(rowIndex, 3, Round(sumSluch.ToString()), StyleCenterBoldNumeric);
                                        efm.PrintCell(rowIndex, 4, Round(sumKU.ToString()), StyleCenterBoldNumeric);
                                        efm.PrintCell(rowIndex, 5, $"", StyleCenterBoldNumeric);
                                        efm.PrintCell(rowIndex, 6, $"", StyleCenterBoldNumeric);
                                        efm.PrintCell(rowIndex, 7, Round(sumSAll.ToString()), StyleCenterBoldNumeric);
                                        amountByCompany += sumSAll;
                                        isFirstPassFile = false;
                                        rowIndex++;
                                        rowIndex++;
                                        break;
                                    case 4:
                                        if (isFirstPassFond)
                                        {
                                            rowIndex += 2;
                                            efm.PrintCell(rowIndex, 1, $"Оплата услуг по направлениям в рамках подушевого финансирования:", StyleCenterBoldText);
                                            efm.AddMergedRegion(new CellRangeAddress(rowIndex, 1, rowIndex, 7));
                                            rowIndex++;
                                            rowIndex++;
                                            efm.PrintCell(rowIndex, 1, $"Код услуги", StyleCenterText);
                                            efm.PrintCell(rowIndex, 2, $"Наименование услуги", StyleCenterText);
                                            efm.PrintCell(rowIndex, 3, $"Кол-во случаев", StyleCenterText);
                                            efm.PrintCell(rowIndex, 4, $"Кол-во услуг (к/дней, п/дней, посещений, УЕТ)", StyleCenterText);
                                            efm.PrintCell(rowIndex, 5, $"Тариф", StyleCenterText);
                                            efm.PrintCell(rowIndex, 6, $"Поправочный коэффициент", StyleCenterText);
                                            efm.PrintCell(rowIndex, 7, $"Стоимость", StyleCenterText);
                                            rowIndex++;
                                            isFirstPassFond = false;
                                        }

                                        sumSluch = 0;
                                        sumKU = 0;
                                        sumSAll = 0;

                                        if (isFirstPassRubric)
                                        {
                                            efm.PrintCell(rowIndex, 1, $"{volumRubric["name"]}", StyleCenterBoldText);
                                            efm.AddMergedRegion(new CellRangeAddress(rowIndex, 1, rowIndex, 7));
                                            rowIndex++;
                                            isFirstPassRubric = false;
                                        }

                                        foreach (var item in listVolumRubric)
                                        {
                                            efm.PrintCell(rowIndex, 1, $"{item["cod"]}", StyleLeftText);
                                            efm.PrintCell(rowIndex, 2, $"{item["name_tarif"]}", StyleLeftText);
                                            efm.PrintCell(rowIndex, 3, Round(item["sluch"].ToString()), StyleCenterNumeric);
                                            efm.PrintCell(rowIndex, 4, Round(item["k_u"].ToString()), StyleCenterNumeric);
                                            efm.PrintCell(rowIndex, 5, Round(item["summa"].ToString()), StyleCenterNumeric);
                                            efm.PrintCell(rowIndex, 6, Round(item["sk"].ToString()), StyleCenterNumeric);
                                            efm.PrintCell(rowIndex, 7, Round(item["s_all"].ToString()), StyleCenterNumeric);

                                            sumSluch += Round(item["sluch"].ToString());
                                            sumKU += Round(item["k_u"].ToString());
                                            sumSAll += Round(item["s_all"].ToString());

                                            rowIndex++;
                                        }

                                        rowIndex++;
                                        efm.PrintCell(rowIndex, 1, $"Всего по группе:", StyleCenterBoldText);
                                        efm.AddMergedRegion(new CellRangeAddress(rowIndex, 1, rowIndex, 2));
                                        efm.PrintCell(rowIndex, 3, Round(sumSluch.ToString()), StyleCenterBoldNumeric);
                                        efm.PrintCell(rowIndex, 4, Round(sumKU.ToString()), StyleCenterBoldNumeric);
                                        efm.PrintCell(rowIndex, 5, $"", StyleCenterBoldNumeric);
                                        efm.PrintCell(rowIndex, 6, $"", StyleCenterBoldNumeric);
                                        efm.PrintCell(rowIndex, 7, Round(sumSAll.ToString()), StyleCenterBoldNumeric);
                                        amountByCompany += sumSAll;
                                        isFirstPassFile = false;
                                        rowIndex++;
                                        rowIndex++;
                                        break;
                                }

                            }
                        }
                        rowIndex++;
                        efm.PrintCell(rowIndex, 2, $"Итого по компании", StyleLeftTextBorderNone);
                        efm.PrintCell(rowIndex, 5, Round(amountByCompany.ToString()), StyleCenterNumericBorderNone);

                        rowIndex += 3;
                        efm.PrintCell(rowIndex, 2, $"Руководитель медицинского учреждения", null);
                        rowIndex++;
                        efm.PrintCell(rowIndex, 3, $"МП", null);
                        rowIndex++;
                        efm.PrintCell(rowIndex, 2, $"Главный бухгалтер медицинского учреждения", null);

                    }
                    catch (Exception ex)
                    {
                        AddLogs(LogType.Error, ex.FullMessage());
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        efm.Save();
                    }
                }

            }

        }
        private void ItogReestrXls(string path, string smoData, DateTime currentDate, CancellationToken token)
        {
            var smoCode = smoData.Split('-')[0];
            var smoName = smoData.Split('-')[1];

            var dtReestr = new DataTable();
            using (var conn = new OracleConnection(connStr))
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
                    var moCode = row["code_mo"].ToString();
                    if (moCode is null)
                        continue;

                    var moName = GetNameMoByCodeMo(moCode);
                    var fileName = $@"{path}\{moCode} Итоговый реестр {smoCode} за {currentDate:MMMM.yyyy}.xlsx";

                    File.Copy(ITOG_REESTR_TEMPLATE, fileName, true);
                    using (var efm = new ExcelOpenXML())
                    {
                        efm.OpenFile(fileName, 0);

                        uint rowIndex = 3;
                        efm.PrintCell(rowIndex, 1, $"медицинских услуг, оказанных {moName}", null);
                        rowIndex += 2;
                        efm.PrintCell(rowIndex, 1, $"гражданам, застрахованным в  {smoName}", null);
                        rowIndex += 2;
                        efm.PrintCell(rowIndex, 1, $"Согласно прилагаемым реестрам, за {currentDate:MMMM} оказаны медицинские услуги на сумму:", null);
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
        private string GetNameMoByCodeMo(string moCode)
        {
            try
            {
                int rIndexDtMo = _dtF003.Rows.IndexOf(_dtF003.Select($" mcod = {moCode}")[0]);
                return Convert.ToString(_dtF003.Rows[rIndexDtMo]["nam_mok"]);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + $"\n\rНе найдена MO {moCode}");
            }
        }
        private void InitF002()
        {
            using (var conn = new OracleConnection(connStr))
            {
                using (var oda = new OracleDataAdapter($"select * from nsi.f002 t where t.tf_okato = '76000' and t.d_end is null", conn))
                {
                    dispatcher.Invoke(() => { Progress1.Text = "Запрос данных f002"; });
                    oda.Fill(_dtF002);
                }
            }
        }
        private void InitF003()
        {
            using (var conn = new OracleConnection(connStr))
            {
                using (var oda = new OracleDataAdapter($"select * from nsi.f003 t  where t.mcod like '750%' and t.d_end is null", conn))
                {
                    dispatcher.Invoke(() => { Progress1.Text = "Запрос данных nsi.f003"; });
                    oda.Fill(_dtF003);
                }
            }
        }
        private void InitVolumRubric()
        {
            using (var conn = new OracleConnection(connStr))
            {
                using (var oda = new OracleDataAdapter($"select * from nsi.volum_rubric t", conn))
                {
                    dispatcher.Invoke(() => { Progress1.Text = "Запрос данных nsi.volum_rubric"; });
                    oda.Fill(_dtVolumRubric);
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
        private decimal Round(string x)
        {
            try
            {
                return Math.Round(decimal.Parse(x.Replace('.', ',')), 2);
            }
            catch (Exception)
            {
                throw new Exception($"Неудается преобразовать в decimal \"{x}\"");
            }
        }

        private readonly DataTable _dtF002 = new DataTable();
        private readonly DataTable _dtF003 = new DataTable();
        private readonly DataTable _dtVolumRubric = new DataTable();
        private readonly int[] _listExistingFond = new int[] { 1, 2, 4 };
        private static string LocalFolder => AppDomain.CurrentDomain.BaseDirectory;
        private string SCHET_FACTURE_TEMPLATE { get; set; } = Path.Combine(LocalFolder, "TEMPLATE", "TEMPLATE_SCHET_FACTURE.xlsx");
        private string ITOG_REESTR_TEMPLATE { get; set; } = Path.Combine(LocalFolder, "TEMPLATE", "TEMPLATE_ITOG_REESTR.xlsx");
        //private string SCHET_FACTURE_MBT_TEMPLATE { get; set; } = Path.Combine(LocalFolder, "TEMPLATE", "TEMPLATE_SCHET_FACTURE_MBT.xlsx");
        //private string ITOG_REESTR_MBT__TEMPLATE { get; set; } = Path.Combine(LocalFolder, "TEMPLATE", "TEMPLATE_ITOG_REESTR_MBT.xlsx");
        private readonly Dispatcher dispatcher;
        private CancellationTokenSource cts;
        private readonly FolderBrowserDialog fbd = new FolderBrowserDialog();
        private readonly string connStr = AppConfig.Property.ConnectionString;
        private DateTime _currentDate;
        private bool _IsOperationRun;
    }
}
