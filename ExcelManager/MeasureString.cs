using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;

namespace ExcelManager
{
    /// <summary>
    /// Измеритель текста
    /// </summary>
    public static class MeasureString
    {
        /// <summary>
        /// Измерить ширину текста
        /// </summary>
        /// <param name="Value">Текст</param>
        /// <param name="familyName">Шрифт</param>
        /// <param name="fontSize">Размер шрифта</param>
        /// <returns></returns>
        public static double MeasureWidth(string Value,string familyName, float fontSize)
        {
            const double cellPadding = .4;
            var normalFont = new Font(familyName, fontSize, GraphicsUnit.Point);
            var bm = new Bitmap(10, 10);
            bm.SetResolution(96, 96);
            using (var g = Graphics.FromImage(bm))
            {
                g.TextRenderingHint = TextRenderingHint.AntiAlias;
                g.PageUnit = GraphicsUnit.Pixel;
                var DesiredWidth = g.MeasureString(Value, normalFont, int.MaxValue, StringFormat.GenericTypographic).Width;
                var point = ((DesiredWidth - 7) / 7d + 1)+ cellPadding;
                return Math.Ceiling(point);
            } 
        }
        /// <summary>
        /// Измерить высоту текста
        /// </summary>
        /// <param name="Value">Текст</param>
        /// <param name="familyName">Шрифт</param>
        /// <param name="fontSize">Размер шрифта</param>
        /// <returns></returns>
        public static double MeasureHeight(string Value, string familyName, float fontSize)
        {
            const double cellPadding = .4;
            var normalFont = new Font(familyName, fontSize, GraphicsUnit.Point);
            var bm = new Bitmap(10, 10);
            bm.SetResolution(96, 96);
            using (var g = Graphics.FromImage(bm))
            {
                g.TextRenderingHint = TextRenderingHint.AntiAlias;
                g.PageUnit = GraphicsUnit.Pixel;
                var DesiredHeight = g.MeasureString(Value, normalFont, int.MaxValue, StringFormat.GenericTypographic).Height;
                var point = (DesiredHeight + cellPadding)*0.75;
                return Math.Ceiling(point);
            }
        }
    }
   


}
