using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
namespace ClientServise
{


   

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

        public static  string N2 = "#,##0.00";
        public static  string N3 = "#,##0.000";
        public static  string N0 = "#,##0";
        //Книга
        HSSFWorkbook hssfworkbook;
        //Лист
        ISheet sheet1;

        public HSSFWorkbook Book
        {
            get
            {
                return hssfworkbook;
            }
        }

        public ExcelFileManager()
        {
            hssfworkbook = new HSSFWorkbook();

            ////create a entry of DocumentSummaryInformation
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "ТФОМС Забайкальского края";
            hssfworkbook.DocumentSummaryInformation = dsi;

            ////create a entry of SummaryInformation
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NesTeRD";
            hssfworkbook.SummaryInformation = si;

            sheet1 = hssfworkbook.CreateSheet("Sheet1");
            
        }


        public ExcelFileManager(bool b)
        {
            hssfworkbook = new HSSFWorkbook();

            ////create a entry of DocumentSummaryInformation
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "ТФОМС Забайкальского края";
            hssfworkbook.DocumentSummaryInformation = dsi;

            ////create a entry of SummaryInformation
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NesTeRD";
            hssfworkbook.SummaryInformation = si;    
            

        }

        public HSSFFont GetFont(short size, string NAME,bool bold)
        {
            HSSFFont hSSFFont = (HSSFFont)hssfworkbook.CreateFont();
            hSSFFont.FontHeightInPoints = size;
            hSSFFont.FontName = NAME;
            if(bold)
                hSSFFont.Boldweight = (short)FontBoldWeight.Bold;
            return hSSFFont;
        }

        public void addScheet(string name)
        {
            sheet1 = hssfworkbook.CreateSheet(name);
            sheet1.PrintSetup.FitHeight = 0;
            sheet1.PrintSetup.FitWidth = 1;
        
        }

        public void SetColumnWidth(int i, int width)
        {
            sheet1.SetColumnWidth(i, width);
        }

        public void SetSheetName(string Name)
        {
            hssfworkbook.SetSheetName(hssfworkbook.GetSheetIndex(sheet1), Name);         
        }

        public ExcelFileManager(string filename,int sheet)
        {
            
            using (FileStream file = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(file);
            }

            ////create a entry of DocumentSummaryInformation
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "ТФОМС Забайкальского края";
            hssfworkbook.DocumentSummaryInformation = dsi;

            ////create a entry of SummaryInformation
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NesTeRD";
            hssfworkbook.SummaryInformation = si;
            
            sheet1 = hssfworkbook.GetSheetAt(sheet);          
        }

     
        public ExcelFileManager(HSSFWorkbook clone)
        {
            hssfworkbook = new HSSFWorkbook();
            ////create a entry of DocumentSummaryInformation
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "ТФОМС Забайкальского края";
            hssfworkbook.DocumentSummaryInformation = dsi;

            ////create a entry of SummaryInformation
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NesTeRD";
            hssfworkbook.SummaryInformation = si;

            for (int i = 0; i < clone.NumberOfSheets; i++)
            {
                HSSFSheet sheetClone = clone.GetSheetAt(i) as HSSFSheet;
                sheetClone.CopyTo(hssfworkbook, sheetClone.SheetName, true, false);
            }

            sheet1 = hssfworkbook.GetSheetAt(0);
        }

        public void InsertRow(int Rownum)
        {
            IRow row =  sheet1.CreateRow(Rownum);
            row.HeightInPoints = 40;
            
        }

        public void CopyRow(int rowSource,int rowto)
        {
            sheet1.ShiftRows(rowto, sheet1.LastRowNum, 1, false, false);
            IRow rowSour = sheet1.GetRow(rowSource);
            IRow newrow = sheet1.CreateRow(rowto);

            newrow.HeightInPoints = rowSour.HeightInPoints;

            var t = rowSour.Sheet.GetMergedRegion(rowSour.Sheet.NumMergedRegions);

            for (int colIndex = 0; colIndex < rowSour.LastCellNum; colIndex++)
            {
                ICell cellSource = rowSour.GetCell(colIndex);
                ICell cellInsert = newrow.CreateCell(colIndex);
                if (cellSource != null)
                {                   
                    cellInsert.CellStyle = cellSource.CellStyle;                    
                }
            }

            for (int i = 0; i < rowSour.Sheet.NumMergedRegions; ++i)
            {
                CellRangeAddress range = rowSour.Sheet.GetMergedRegion(i);
                if (range.FirstRow == rowSource && range.LastRow == rowSource)
                {
                    CellRangeAddress cra = new CellRangeAddress(rowto, rowto, range.FirstColumn, range.LastColumn);
                    newrow.Sheet.AddMergedRegion(cra);
                }
            }
        }



        public void RenameActivScheet(string Name)
        {
            hssfworkbook.SetSheetName(hssfworkbook.GetSheetIndex(sheet1), Name);
        }
        public void SetActivSheet(int index)
        {
            sheet1 = hssfworkbook.GetSheetAt(index);
        }

        public int AddScheet(string Name)
        {
            
            return hssfworkbook.GetSheetIndex(hssfworkbook.CreateSheet(Name));
        }


        public void PrintCell(int Row, int Column, string value, ICellStyle style)
        {
            IRow row = sheet1.GetRow(Row);

            if (row == null)
            {
                row = sheet1.CreateRow(Row);
            }

            ICell cell = row.GetCell(Column);
            if (cell == null)
            {
                cell = row.CreateCell(Column);
            }
            if (style != null)
            {
                cell.CellStyle = style;
            }
            
             cell.SetCellValue(value);
           
            
        }

        public void PrintHyperlink(int Row, int Column, string path)
        {
            IRow row = sheet1.GetRow(Row);
            if (row == null)
                row = sheet1.CreateRow(Row);
            ICell cell = row.CreateCell(Column);
            cell.SetCellValue("TEST FILE");
            cell.Hyperlink = new HSSFHyperlink(HyperlinkType.File);
            cell.Hyperlink.Address = path;
     

        }

        public void AddMergedRegion(int RowStart,int ColStart,int RowEnd,int ColEnd)
        {
            sheet1.AddMergedRegion(new CellRangeAddress(RowStart, RowEnd,ColStart, ColEnd));            
        }

        public void AddMergedRegion(int RowStart, int ColStart, int RowEnd, int ColEnd,ICellStyle style)
        {
            for (int i = RowStart; i <= RowEnd; i++)
            {
                for (int j = ColStart; j <= ColEnd; j++)
                {
                    IRow row = sheet1.GetRow(i);

                    if (row == null)
                    {
                        row = sheet1.CreateRow(i);
                    }

                    ICell cell = row.GetCell(j);
                    if (cell == null)
                    {
                        cell = row.CreateCell(j);
                    }
                    cell.CellStyle = style;
                }
            }
            sheet1.AddMergedRegion(new CellRangeAddress(RowStart, RowEnd, ColStart, ColEnd));
        }
        /// <summary>
        /// Изменение высоты строки
        /// </summary>
        /// <param name="RowIndex">Строка</param>
        /// <param name="size">Высота</param>
        public void SizeRows(int RowIndex, float size)
        {
            IRow row = sheet1.GetRow(RowIndex);
            if (row == null)
                row = sheet1.CreateRow(RowIndex);
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
            NPOI.SS.Util.CellRangeAddress CRA = new NPOI.SS.Util.CellRangeAddress(Row, RowEnd, Column, ColumnEnd);
            for (int R = Row; R <= RowEnd; R++)
            {
                for (int C = Column; C <= ColumnEnd; C++)
                {
                    IRow row = sheet1.GetRow(R);
                    if (row == null)
                        row = sheet1.CreateRow(R);

                    ICell cell = row.GetCell(C);
                    if(cell == null)
                        cell = row.CreateCell(C);

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
            IRow row = sheet1.GetRow(Row);
            if (row == null)
            {
                row = sheet1.CreateRow(Row);
            }

            
            ICell cell = row.GetCell(Column);
            if (cell == null)
            {
                cell = row.CreateCell(Column);
            }
            if (style != null)
            {
                cell.CellStyle = style;
            }
            cell.SetCellValue(value); 
            
        }

        public void SetBorder(int rowstart, int columnstart, int rowend, int columnend,ICellStyle template)
        {
            for (int i = rowstart; i < rowend; i++)
            {
                IRow row = sheet1.GetRow(i);
                if (row == null)
                    row = sheet1.CreateRow(i);

                for (int j = columnstart; j < columnend; j++)
                {
                    ICell cell = row.GetCell(j);
                    if (cell == null)
                    {
                        cell = row.CreateCell(j);
                        cell.CellStyle = template;
                    }
                    ICellStyle style = cell.CellStyle;
                    style.BorderBottom = BorderStyle.Thin;
                    style.BorderLeft = BorderStyle.Thin;
                    style.BorderRight = BorderStyle.Thin;
                    style.BorderTop = BorderStyle.Thin;
                }
            }
        }
        /// <summary>
        /// Получить формат
        /// </summary>
        /// <param name="Borders">Наличие граней</param>
        /// <param name="CA">Выравнивание</param>
        /// <param name="DataFormat">Формат данных Excel </param>
        /// <returns>Формат</returns>
        public ICellStyle GetStyle(bool Borders, CellAlignment CA, string DataFormat)
        {
            ICellStyle res = hssfworkbook.CreateCellStyle();
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
            if (DataFormat != "")
            {
                IDataFormat format = hssfworkbook.CreateDataFormat();  //res.DataFormat = format.GetFormat("#,##0,");
                res.DataFormat = format.GetFormat(DataFormat);
            }
            return res;
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
            ICellStyle res = hssfworkbook.CreateCellStyle();
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

            if (DataFormat != "")
            {
                IDataFormat format = hssfworkbook.CreateDataFormat();  //res.DataFormat = format.GetFormat("#,##0,");
                res.DataFormat = format.GetFormat(DataFormat);
            }
            res.WrapText = WrapText;
            if (BoldFont)
            {
                IFont font = hssfworkbook.CreateFont();
                font.Boldweight = (short)FontBoldWeight.Bold;
                res.SetFont(font);
            }
            return res;
        }

        public ICellStyle GetStyle(bool Borders, CellAlignment CA, string DataFormat, bool BoldFont, bool WrapText,NPOI.HSSF.Util.HSSFColor color)
        {
            ICellStyle res = hssfworkbook.CreateCellStyle();
            if (Borders)
            {
                res.BorderBottom = BorderStyle.Thin;
                res.BorderLeft = BorderStyle.Thin;
                res.BorderRight = BorderStyle.Thin;
                res.BorderTop = BorderStyle.Thin;
            }
            switch (CA)
            {
                case CellAlignment.CENTER: res.Alignment = HorizontalAlignment.Center; res.VerticalAlignment = VerticalAlignment.Center; break;
                case CellAlignment.LEFT: res.Alignment = HorizontalAlignment.Left; res.VerticalAlignment = VerticalAlignment.Center; break;
                case CellAlignment.RIGHT: res.Alignment = HorizontalAlignment.Right; res.VerticalAlignment = VerticalAlignment.Center; break;
                default:
                    break;
            }

            if (DataFormat != "")
            {
                IDataFormat format = hssfworkbook.CreateDataFormat();  //res.DataFormat = format.GetFormat("#,##0,");
                res.DataFormat = format.GetFormat(DataFormat);
            }
            res.WrapText = WrapText;
            if (BoldFont)
            {
                IFont font = hssfworkbook.CreateFont();
                font.Boldweight = (short)FontBoldWeight.Bold;
                res.SetFont(font);
            }


            res.FillForegroundColor = color.Indexed;
            res.FillPattern  = FillPattern.SolidForeground;

            return res;
        }
        /// <summary>
        /// Выровнить столбцы
        /// </summary>
        /// <param name="start">Первый столбец</param>
        /// <param name="end">Второй столбец</param>
        public void AutoSizeColumn(int start,int end)
        {
            for (int i = start; i < end; i++)
            {                
                sheet1.AutoSizeColumn(i, true);
            }
        }
        /// <summary>
        /// Сохранить в файл 
        /// </summary>
        /// <param name="path">Имя файла</param>
        public void SaveToFile(string path)
        {
            //Write the stream data of workbook to the root directory
            FileStream file = new FileStream(path, FileMode.Create);
            hssfworkbook.Write(file);
            file.Close();
        }
        /// <summary>
        /// Открыть файл в программе Excel
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        static public void OpenFile(string path)
        {    
            Microsoft.Office.Interop.Excel.Application App = new Microsoft.Office.Interop.Excel.Application(); 
            Microsoft.Office.Interop.Excel.Workbooks workbooks = App.Workbooks; 
            Microsoft.Office.Interop.Excel.Workbook book = null; 
            book = workbooks.Open(path, 1, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "", false, false, 0, true, false, Microsoft.Office.Interop.Excel.XlCorruptLoad.xlNormalLoad);
            
            

                        
            App.Visible = true;

            //App.GetSaveAsFilename(Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
        }

        public void AddMergedRegion(CellRangeAddress cra)
        {
            sheet1.AddMergedRegion(cra);
        }

    }

   


}
