using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.Util;

namespace ServiceLoaderMedpomData
{/*
    /// <summary>
    /// Выравнивание в ячейке
    /// </summary>
    public enum CellAlignment
    {
        CENTER = 0,
        RIGHT = 1,
        LEFT = 2
    }
    /// <summary>
    /// Менеджер работы с Excel документами
    /// </summary>
    public class ExcelFileManager
    {
        //Книга
        HSSFWorkbook hssfworkbook;
        //Лист
        ISheet sheet1;
        public ExcelFileManager(string filename, int sheet)
        {

            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(file);
            }
            sheet1 = hssfworkbook.GetSheetAt(sheet);
        }

        public string CurrentSheetName => sheet1.SheetName;

        public ExcelFileManager(string SheetName)
        {
            hssfworkbook = new HSSFWorkbook();

            ////create a entry of DocumentSummaryInformation
            var dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "ТФОМС Забайкальского края";
            hssfworkbook.DocumentSummaryInformation = dsi;

            ////create a entry of SummaryInformation
            var si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NesTeRD";
            hssfworkbook.SummaryInformation = si;
            sheet1 = hssfworkbook.CreateSheet(SheetName);
        }

        public void SetActivSheet(int index)
        {
            sheet1 = hssfworkbook.GetSheetAt(index);
        }
        public ExcelFileManager()
        {

         
            hssfworkbook = new HSSFWorkbook();
            
            ////create a entry of DocumentSummaryInformation
            var dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "ТФОМС Забайкальского края";
            
            hssfworkbook.DocumentSummaryInformation = dsi;

            ////create a entry of SummaryInformation
            var si = PropertySetFactory.CreateSummaryInformation();
            si.Author = "Нестеренок Д.В.";
            hssfworkbook.SummaryInformation = si;

            sheet1 = hssfworkbook.CreateSheet("Sheet1");
            
        }
        /// <summary>
        /// Напечатать ячейку
        /// </summary>
        /// <param name="Row">Индекс строки</param>
        /// <param name="Column">Индекс столбца</param>
        /// <param name="value">Значение строка</param>
        /// <param name="style">Стиль ячейки</param>
        public void PrintCell(int Row, int Column, string value, ICellStyle style)
        {
            var row = sheet1.GetRow(Row) ?? sheet1.CreateRow(Row);

            var cell = row.CreateCell(Column);
             cell.CellStyle = style;
            
             cell.SetCellValue(value);
           
            
        }
        /// <summary>
        /// Изменение высоты строки
        /// </summary>
        /// <param name="RowIndex">Строка</param>
        /// <param name="size">Высота</param>
        public void SizeRows(int RowIndex, float size)
        {
            var row = sheet1.GetRow(RowIndex) ?? sheet1.CreateRow(RowIndex);
            row.HeightInPoints = size;
        }
        /// <summary>
        /// Изменение формата для региона
        /// </summary>
        /// <param name="Row">Начало строка</param>
        /// <param name="Column">Начало столбец</param>
        /// <param name="RowEnd">Конец строка</param>
        /// <param name="ColumnEnd">Конец столбец</param>
        /// <param name="style">Формат</param>
        public void SpanCell(int Row, int Column, int RowEnd, int ColumnEnd, ICellStyle style)
        {
            var CRA = new NPOI.SS.Util.CellRangeAddress(Row, RowEnd, Column, ColumnEnd);
            for (var R = Row; R <= RowEnd; R++)
            {
                for (var C = Column; C <= ColumnEnd; C++)
                {
                    var row = sheet1.GetRow(R) ?? sheet1.CreateRow(R);
                    var cell = row.GetCell(C) ?? row.CreateCell(C);
                    cell.CellStyle = style;

                }
            }
            sheet1.AddMergedRegion(CRA);    
            
        }
        /// <summary>
        /// Напечатать ячейку
        /// </summary>
        /// <param name="Row">Индекс строки</param>
        /// <param name="Column">Индекс столбца</param>
        /// <param name="value">Значение число</param>
        /// <param name="style">Стиль ячейки</param>
        public void PrintCell(int Row, int Column, double value, ICellStyle style)
        {
            var row = sheet1.GetRow(Row) ?? sheet1.CreateRow(Row);
            var cell = row.CreateCell(Column);
            
            cell.CellStyle = style;
            cell.SetCellValue(value); 
            
        }

        /// <summary>
        /// Получить формат
        /// </summary>
        /// <param name="Borders">Наличие граней</param>
        /// <param name="CA">Выравнивание</param>
        /// <param name="DataFormat">Формат данных Excel </param>
        /// <returns>Формат</returns>
        public ICellStyle GetStyle(bool Borders,CellAlignment CA,string DataFormat)
        {
            var res = hssfworkbook.CreateCellStyle();
            if (Borders)
            {
                res.BorderBottom = BorderStyle.Thin;
                res.BorderLeft = BorderStyle.Thin;
                res.BorderRight = BorderStyle.Thin;
                res.BorderTop = BorderStyle.Thin;
            }
            switch(CA)
            {
                case CellAlignment.CENTER: res.Alignment = HorizontalAlignment.Center; res.VerticalAlignment = VerticalAlignment.Center; break;
                case CellAlignment.LEFT: res.Alignment = HorizontalAlignment.Left; res.VerticalAlignment = VerticalAlignment.Center; break;
                case CellAlignment.RIGHT: res.Alignment = HorizontalAlignment.Right; res.VerticalAlignment = VerticalAlignment.Center; break;
                default:
                    break;
            }
            var format = hssfworkbook.CreateDataFormat();  //res.DataFormat = format.GetFormat("#,##0,");
            res.DataFormat = format.GetFormat(DataFormat);   
            return res;
        }

        /// <summary>
        /// Установить цвета 100 - красный
        /// 101 - зеленый
        /// 102 - желтый
        /// </summary>
        public HSSFColor RedColor
        {
            get
            {
                var palette = ((NPOI.HSSF.UserModel.HSSFWorkbook)hssfworkbook).GetCustomPalette();
                HSSFColor t = null;
                try
                { 
                  t = palette.FindColor(230, 185, 184);
                }
                catch 
                {

                }
                if (t == null)
                {
                    palette.SetColorAtIndex(40, 230, 185, 184);
                    return palette.GetColor(40);
                }
                return t;
                   
            }

        }

        public HSSFColor GreenColor
        {
            get
            {
                var palette = ((NPOI.HSSF.UserModel.HSSFWorkbook)hssfworkbook).GetCustomPalette();
                HSSFColor t = null;
                try
                {
                     t = palette.FindColor(148, 208, 80);
                }
                catch
                {

                }
                if (t == null)
                {
                    palette.SetColorAtIndex(41, 148, 208, 80);
                    return palette.GetColor(41);
                }
                return t;
            }

        }

        public HSSFColor YellowColor
        {
            get
            {
                var palette = ((NPOI.HSSF.UserModel.HSSFWorkbook)hssfworkbook).GetCustomPalette();
                HSSFColor t = null;
                try
                {
                     t = palette.FindColor(255, 255, 153);
                }
                catch
                {

                }
                if (t == null)
                {
                    palette.SetColorAtIndex(42, 255, 255, 153);
                    return palette.GetColor(42);
                }
                return t;
            }

        }
        /// <summary>
        /// Получить формат
        /// </summary>
        /// <param name="Borders">Наличие граней</param>
        /// <param name="CA">Выравнивание</param>
        /// <param name="DataFormat">Формат данных excell</param>
        /// <param name="BoldFont">Шрифт Bold</param>
        /// <param name="WrapText">Перенос по словам</param>
        /// <returns>Формат</returns>
        public ICellStyle GetStyle(bool Borders, CellAlignment CA, string DataFormat, bool BoldFont, bool WrapText)
        {
            var res = hssfworkbook.CreateCellStyle();
            if (Borders)
            {
                res.BorderBottom = BorderStyle.Thin;
                res.BorderLeft = BorderStyle.Thin;
                res.BorderRight = BorderStyle.Thin;
                res.BorderTop = BorderStyle.Thin;
            }
            switch(CA)
            {
                case CellAlignment.CENTER: res.Alignment = HorizontalAlignment.Center; res.VerticalAlignment = VerticalAlignment.Center; break;
                case CellAlignment.LEFT: res.Alignment = HorizontalAlignment.Left; res.VerticalAlignment = VerticalAlignment.Center; break;
                case CellAlignment.RIGHT: res.Alignment = HorizontalAlignment.Right; res.VerticalAlignment = VerticalAlignment.Center; break;
                default:
                    break;
            }
            
            var format = hssfworkbook.CreateDataFormat();  //res.DataFormat = format.GetFormat("#,##0,");
            res.DataFormat = format.GetFormat(DataFormat);
            res.WrapText = WrapText;
            if (BoldFont)
            {
                var font = hssfworkbook.CreateFont();
                font.Boldweight = (short)FontBoldWeight.Bold;
                res.SetFont(font);
            }
            return res;
        }
        /// <summary>
        /// Выровнить столбцы
        /// </summary>
        /// <param name="start">Первый столбец</param>
        /// <param name="end">Второй столбец</param>
        public void AutoSizeColumn(int start,int end)
        {
            for (var i = start; i < end; i++)
            {                
                sheet1.AutoSizeColumn(i, true);
            }
            
        }

       

        public void SetColumnWidth(int colIdx, double widthInTwips)
        {

            sheet1.SetColumnWidth(colIdx, (int)(441.3793d + 256d * (widthInTwips - 1d)));

        }
        /// <summary>
        /// Сохранить в файл 
        /// </summary>
        /// <param name="path">Имя файла</param>
        public void SaveToFile(string path)
        {            
            //Write the stream data of workbook to the root directory
            var file = new FileStream(path, FileMode.Create);
            hssfworkbook.Write(file);
            file.Close();
            
           
        }
        /// <summary>
        /// Открыть файл в программе Excel
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        static public void OpenFile(string path)
        {    
            var App = new Microsoft.Office.Interop.Excel.Application(); 
            var workbooks = App.Workbooks; 
            Microsoft.Office.Interop.Excel.Workbook book = null; 
            book = workbooks.Open(path, 1, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "", false, false, 0, true, false, Microsoft.Office.Interop.Excel.XlCorruptLoad.xlNormalLoad);
            
            

                        
            App.Visible = true;

            //App.GetSaveAsFilename(Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
        }
        /// <summary>
        /// Добавить лист в конец и сделать его активным
        /// </summary>
        public void AddSheet(string SheetName)
        {
            sheet1 =  hssfworkbook.CreateSheet(SheetName);
        }

        public void AddMergedRegion(CellRangeAddress cra)
        {
            sheet1.AddMergedRegion(cra);

        }

        /// <summary>
        /// Добавить лист в начало и сделать его активным
        /// </summary>
        public void InsertSheet(string SheetName)
        {
            sheet1 = hssfworkbook.CreateSheet(SheetName);
            hssfworkbook.SetSheetOrder(SheetName, 0);
        }


    }

   */






}
