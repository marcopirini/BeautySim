
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using BeautySim2023.DataModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace BeautySim2023
{
    /******************************************
     * Class name: ERM
     * Author:
     * Creation:
     * Last modify:
     * Version:
     *
     * DESCRIPTION
     *
     * *****************************************/

    public class ResultManager
    {
        public const string Separator = "$%$";

        public ResultManager()
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        public delegate void OnImpossibleSavePdf(string path);

        public static System.Drawing.Image ResizeImage(System.Drawing.Image image, System.Drawing.Size size, bool preserveAspectRatio = true)
        {
            int newWidth;
            int newHeight;

            newWidth = size.Width;
            newHeight = size.Height;

            System.Drawing.Image newImage = new Bitmap(newWidth, newHeight);
            using (Graphics graphicsHandle = Graphics.FromImage(newImage))
            {
                graphicsHandle.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphicsHandle.DrawImage(image, 0, 0, newWidth, newHeight);
            }
            return newImage;
        }

        #region METODI CREAZIONE REPORT PDF

        private static List<ObjectRecord> objectsRecord;

        public static string PdfFile { get; private set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="objRec"></param>
        /// <param name="reportsUnifiedOrNot"></param>
        /// <param name="whereToSave"></param>
        public static void CreateReportPDF(List<Results> objRec, string whereToSave)
        {
            objectsRecord = new List<ObjectRecord>();
            Document doc = new Document();
            doc.Info.Author = BeautySim.Globalization.Language.str_report_author;
            doc.Info.Subject = BeautySim.Globalization.Language.str_report_subject;
            doc.Info.Title = BeautySim.Globalization.Language.str_report_title;

            DefineStyles_PDF(doc);

            DefineCover_PDF(doc);

            MigraDoc.DocumentObjectModel.Shapes.Image image2 = doc.LastSection.AddImage("Images\\BTY.png");

            image2.Width = "9cm";
            image2.Left = ShapePosition.Center;

            foreach (Results obr in objRec)
            {
                objectsRecord.Add(new ObjectRecord(obr));
            }

            for (int kk = 0; kk < objectsRecord.Count; kk++)
            {
                AddEntityInformation_PDF(doc, objectsRecord[kk]);
                if (kk != (objectsRecord.Count - 1))
                {
                    doc.LastSection.AddPageBreak();
                }
            }

            DateTime now = DateTime.Now;
            string pdf_name = string.Format("{0:00}{1:00}{2:00}_{3:00}{4:00}{5:00}_ACTIVITIES_REPORT.pdf", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);

            SaveReport_PDF(doc, whereToSave, pdf_name);
        }

        #endregion METODI CREAZIONE REPORT PDF

        #region FILLING METHODS PDF

        private static string MigraDocFilenameFromByteArray(byte[] image)
        {
            return "base64:" +
            Convert.ToBase64String(image);
        }

        private static void AddEntityInformation_PDF(Document doc, ObjectRecord obr)
        {
            try
            {
                ResultToSave cc = ResultToSave.Load<ResultToSave>(obr.Result.FilePath);

                Paragraph paragraph = doc.LastSection.AddParagraph(BeautySim.Globalization.Language.str_report_paragraph, "Heading1");
                paragraph.Format.Alignment = ParagraphAlignment.Center;
                paragraph = doc.LastSection.AddParagraph(BeautySim.Globalization.Language.str_student + " " + obr.Student.Title + " " + obr.Student.Name + " " + obr.Student.Surname + "; " + obr.Student.Organization, "NormalBigger");
                paragraph = doc.LastSection.AddParagraph(BeautySim.Globalization.Language.str_teacher + " " + obr.Teacher.Title + " " + obr.Teacher.Name + " " + obr.Teacher.Surname + "; " + obr.Teacher.Organization, "NormalBigger");
                paragraph = doc.LastSection.AddParagraph(BeautySim.Globalization.Language.str_event + " " + (obr.EventOrg != null ? (obr.EventOrg.CompleteName + ", " + obr.EventOrg.Location) : BeautySim.Globalization.Language.str_no_evt) + ", " + obr.DateTimeResult, "NormalBigger");
                paragraph = doc.LastSection.AddParagraph(BeautySim.Globalization.Language.str_case + " " + obr.NameCase, "NormalBigger");
                paragraph = doc.LastSection.AddParagraph(BeautySim.Globalization.Language.str_glob_score + " " + obr.Result.Score.ToString("00.00") + "/100", "NormalBigger");

                paragraph.Format.SpaceAfter = "10";
                AddSpecificResult_PDF(doc, cc, obr, Enum_ResultType.GENERICRESULTS, -1);

            }
            catch (Exception)
            {
            }
        }

        private static void AddSpecificResult_PDF(Document doc, ResultToSave cc, ObjectRecord h, Enum_ResultType tableType, int indexItem)
        {
            //Paragraph p = new Paragraph();
            //if (cc != null)
            //{
            //    if (tableType == Enum_ResultType.GENERICRESULTS)
            //    {
            //        //NAME ITEM | VALUE | POINTS OR COMMENT
            //        p = doc.LastSection.AddParagraph(BeautySim.Globalization.Language.str_params_of_case, "TextBox");

            //        Table table = new Table();
            //        table.Format.Alignment = ParagraphAlignment.Center;
            //        table.Borders.Width = 0.2;

            //        Column column = table.AddColumn(Unit.FromCentimeter(5)); column.Format.Alignment = ParagraphAlignment.Center;
            //        table.AddColumn(Unit.FromCentimeter(4.2)); column.Format.Alignment = ParagraphAlignment.Center;
            //        table.AddColumn(Unit.FromCentimeter(7)); column.Format.Alignment = ParagraphAlignment.Center;

            //        Row row = table.AddRow(); row.VerticalAlignment = VerticalAlignment.Center;
            //        row.Shading.Color = Colors.LightBlue;
            //        Cell cell = row.Cells[0]; p = cell.AddParagraph(BeautySim.Globalization.Language.str_item); p.Style = "TableHeader";
            //        cell = row.Cells[1]; p = cell.AddParagraph(BeautySim.Globalization.Language.str_value); p.Style = "TableHeader";
            //        cell = row.Cells[2]; p = cell.AddParagraph(BeautySim.Globalization.Language.str_assessment); p.Style = "TableHeader";

            //        if (cc.IsMultipleNeedleInjections)
            //        {
            //            // Nerve targeted
            //            row = table.AddRow(); row.VerticalAlignment = VerticalAlignment.Center;
            //            cell = row.Cells[0]; p = cell.AddParagraph(BeautySim.Globalization.Language.str_report_target_nerves); p.Style = "TableHeader";
            //            cell = row.Cells[1]; p = cell.AddParagraph(cc.TotalTargetInjected.ToString() + " / " + cc.TotalTargetNerves.ToString()); p.Style = "TableHeader";
            //            cell = row.Cells[2]; p = cell.AddParagraph(BeautySim.Globalization.Language.str_report_score_target_explanation); p.Style = "TableHeader";

            //            // Wrong Injections
            //            row = table.AddRow(); row.VerticalAlignment = VerticalAlignment.Center;
            //            cell = row.Cells[0]; p = cell.AddParagraph(BeautySim.Globalization.Language.str_report_total_wrong_injections); p.Style = "TableHeader";
            //            cell = row.Cells[1]; p = cell.AddParagraph(cc.WrongInjections.ToString() ); p.Style = "TableHeader";
            //            cell = row.Cells[2]; p = cell.AddParagraph(BeautySim.Globalization.Language.str_report_score_targetwrongInjection_explanation); p.Style = "TableHeader";

            //            if (cc.TotalNervesWrongHits>0)
            //            {
            //                row = table.AddRow(); row.VerticalAlignment = VerticalAlignment.Center;
            //                cell = row.Cells[0]; p = cell.AddParagraph(BeautySim.Globalization.Language.str_report_wrong_nerve_hits); p.Style = "TableHeader";
            //                cell = row.Cells[1]; p = cell.AddParagraph(cc.TotalNervesWrongHits.ToString()); p.Style = "TableHeader";
            //                cell = row.Cells[2]; p = cell.AddParagraph(BeautySim.Globalization.Language.str_report_hit + " " + cc.TotalNervesWrongHits.ToString() + " " + BeautySim.Globalization.Language.str_report_score_targetwrongnerveareas_explanation) ; p.Style = "TableHeader";
            //            }

            //            if (cc.TotalVascularHits > 0)
            //            {
            //                row = table.AddRow(); row.VerticalAlignment = VerticalAlignment.Center;
            //                cell = row.Cells[0]; p = cell.AddParagraph(BeautySim.Globalization.Language.str_report_wrong_vascular_hits); p.Style = "TableHeader";
            //                cell = row.Cells[1]; p = cell.AddParagraph(cc.TotalVascularHits.ToString()); p.Style = "TableHeader";
            //                cell = row.Cells[2]; p = cell.AddParagraph(BeautySim.Globalization.Language.str_report_hit + " " + cc.TotalVascularHits.ToString() + " "+ BeautySim.Globalization.Language.str_report_score_targetwrongvascularareas_explanation); p.Style = "TableHeader";
            //            }
            //        }
            //        else
            //        {
            //            // FASCIA PUNTURED
            //            row = table.AddRow(); row.VerticalAlignment = VerticalAlignment.Center;
            //            cell = row.Cells[0]; p = cell.AddParagraph(BeautySim.Globalization.Language.str_fascia_targeted); p.Style = "TableHeader";
            //            cell = row.Cells[1]; p = cell.AddParagraph(cc.FasciaPuntured ? BeautySim.Globalization.Language.str_yes : BeautySim.Globalization.Language.str_no); p.Style = "TableHeader";
            //            cell = row.Cells[2]; p = cell.AddParagraph(cc.FasciaPuntured ? BeautySim.Globalization.Language.str_ok : BeautySim.Globalization.Language.str_score_set_to_zero); p.Style = "TableHeader";
            //        }



            //        // NEEDLE INSERTIONS
            //        row = table.AddRow(); row.VerticalAlignment = VerticalAlignment.Center;
            //        cell = row.Cells[0]; p = cell.AddParagraph(BeautySim.Globalization.Language.str_needle_insertions); p.Style = "TableHeader";
            //        cell = row.Cells[1]; p = cell.AddParagraph(cc.NumberOfNeedleInsertions.ToString("00")); p.Style = "TableHeader";
            //        string comment = BeautySim.Globalization.Language.str_ok;
            //        if (cc.NumberOfNeedleInsertions == 0)
            //        {
            //            comment = BeautySim.Globalization.Language.str_anomaly;
            //        }
            //        if (cc.IsMultipleNeedleInjections)
            //        {
            //            if (cc.NumberOfNeedleInsertions > cc.TotalTargetNerves)
            //            {
            //                comment = BeautySim.Globalization.Language.str_too_many_ins + " " + (10 * (cc.NumberOfNeedleInsertions - cc.TotalTargetNerves)).ToString();
            //            }
            //        }
            //        else
            //        {
            //            if (cc.NumberOfNeedleInsertions > 1)
            //            {
            //                comment = BeautySim.Globalization.Language.str_too_many_ins + " " + (10 * (cc.NumberOfNeedleInsertions - 1)).ToString();
            //            }
            //        }
            //        cell = row.Cells[2]; p = cell.AddParagraph(comment); p.Style = "TableHeader";

            //        // TIME DISTANCE
            //        row = table.AddRow(); row.VerticalAlignment = VerticalAlignment.Center;
            //        cell = row.Cells[0]; p = cell.AddParagraph(BeautySim.Globalization.Language.str_exec_time); p.Style = "TableHeader";
            //        TimeSpan c = TimeSpan.FromSeconds(cc.TimeNeedlePhase);
            //        cell = row.Cells[1]; p = cell.AddParagraph(c.Minutes.ToString() + ":" + c.Seconds.ToString()); p.Style = "TableHeader";
            //        comment = BeautySim.Globalization.Language.str_within_range;
            //        if (c.Minutes > 2)
            //        {
            //            comment = BeautySim.Globalization.Language.str_too_lengthy + " " + (5 * (c.Minutes - 2)).ToString();

            //        }
            //        else
            //        {
            //            comment = BeautySim.Globalization.Language.str_lt_3min;
            //        }
            //        cell = row.Cells[2]; p = cell.AddParagraph(comment); p.Style = "TableHeader";



            //        doc.LastSection.Add(table);
            //    }
            //}
        }

        private static MigraDoc.DocumentObjectModel.Color colorOK = new MigraDoc.DocumentObjectModel.Color(170, 0, 150, 136);
        private static MigraDoc.DocumentObjectModel.Color colorWrong = new MigraDoc.DocumentObjectModel.Color(170, 219, 76, 76);

        private static string ConvertStringToReadableFormat_PDF(string p)
        {
            string toRet = p;
            string[] splitted = p.Split('.');

            if (splitted.Count() > 1)
            {
                if (splitted[1].Length > 2)
                {
                    toRet = splitted[0] + "." + splitted[1].Substring(0, 2);
                }
            }

            return toRet;
        }

        private static void DefineCover_PDF(Document document)
        {
            Section section = document.AddSection();

            document.LastSection.PageSetup.OddAndEvenPagesHeaderFooter = true;
            document.LastSection.PageSetup.StartingNumber = 1;
            //document.LastSection.PageSetup. = 1;

            HeaderFooter headerPrimary = document.LastSection.Headers.Primary;
            HeaderFooter headerEven = document.LastSection.Headers.EvenPage;
            //HeaderFooter footer = document.LastSection.Footers.Primary;

            MigraDoc.DocumentObjectModel.Shapes.Image image1 = headerPrimary.AddImage("Images\\logoAccurate.png");
            image1.Width = "4cm";
            image1.Left = ShapePosition.Center;

            MigraDoc.DocumentObjectModel.Shapes.Image image2 = headerEven.AddImage("Images\\logoAccurate.png");
            image2.Width = "4cm";
            image2.Left = ShapePosition.Center;

            // Create a paragraph with centered page number. See definition of style "Footer".
            Paragraph paragraph = new Paragraph();
            paragraph.AddTab();
            paragraph.AddPageField();

            // Add paragraph to footer for odd pages.
            document.LastSection.Footers.Primary.Add(paragraph);
            document.LastSection.Footers.EvenPage.Add(paragraph.Clone());
        }

        private static void DefineStyles_PDF(Document document)
        {
            // Get the predefined style Normal.
            Style style = document.Styles["Normal"];

            // Because all styles are derived from Normal, the next line changes the
            // font of the whole document. Or, more exactly, it changes the font of
            // all styles and paragraphs that do not redefine the font.
            style.Font.Name = "Arial Unicode MS";

            // Heading1 to Heading9 are predefined styles with an outline level. An outline level
            // other than OutlineLevel.BodyText automatically creates the outline (or bookmarks)
            // in PDF.

            style = document.Styles["Heading1"];
            style.Font.Size = 14;
            style.Font.Bold = true;
            style.Font.Color = Colors.DarkBlue;
            style.ParagraphFormat.PageBreakBefore = false;
            style.ParagraphFormat.SpaceBefore = 6;
            style.ParagraphFormat.SpaceAfter = 6;

            style = document.Styles["Heading2"];
            style.Font.Size = 12;
            style.Font.Bold = true;
            style.ParagraphFormat.PageBreakBefore = false;
            style.ParagraphFormat.SpaceBefore = 6;
            style.ParagraphFormat.SpaceAfter = 6;

            style = document.Styles["Heading3"];
            style.Font.Size = 10;
            style.Font.Bold = true;
            style.Font.Italic = true;
            style.ParagraphFormat.SpaceBefore = 6;
            style.ParagraphFormat.SpaceAfter = 3;

            style = document.Styles.AddStyle("NormalBigger", "Normal");
            style.Font.Bold = true;
            style.Font.Italic = true;
            //style.ParagraphFormat.SpaceBefore = 2;
            //style.ParagraphFormat.SpaceAfter = 2;
            //style.ParagraphFormat.Alignment = ParagraphAlignment.Center;

            style = document.Styles.AddStyle("TableHeader", "Normal");
            style.Font.Size = 7;
            style.Font.Bold = true;
            style.Font.Italic = true;
            style.ParagraphFormat.SpaceBefore = 2;
            style.ParagraphFormat.SpaceAfter = 2;
            style.ParagraphFormat.Alignment = ParagraphAlignment.Center;

            style = document.Styles.AddStyle("TableContent", "Normal");
            style.Font.Size = 7;
            style.Font.Bold = false;
            style.Font.Italic = false;
            style.ParagraphFormat.SpaceBefore = 1.5;
            style.ParagraphFormat.SpaceAfter = 1.5;
            style.ParagraphFormat.Alignment = ParagraphAlignment.Center;

            style = document.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);

            style = document.Styles[StyleNames.Footer];
            style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

            // Create a new style called TextBox based on style Normal
            style = document.Styles.AddStyle("TextBox", "Normal");
            style.ParagraphFormat.Alignment = ParagraphAlignment.Justify;
            //style.ParagraphFormat.Borders.Width = 2.5;
            style.ParagraphFormat.Borders.Distance = "3pt";
            //TODO: Colors
            style.ParagraphFormat.Shading.Color = Colors.SkyBlue;

            // Create a new style called TOC based on style Normal
            style = document.Styles.AddStyle("TOC", "Normal");
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right, TabLeader.Dots);
            style.ParagraphFormat.Font.Color = Colors.Blue;

            style = document.Styles.AddStyle("XTickLabels", "Normal");
            style.Font.Size = 7;
        }

        private static void SaveReport_PDF(Document doc, string whereToSave, string nameSpecific)
        {
            try
            {
                PdfDocumentRenderer renderer = new PdfDocumentRenderer(true, PdfSharp.Pdf.PdfFontEmbedding.Always);

                renderer.Document = doc;
                renderer.RenderDocument();

                PdfFile = whereToSave + "\\" + nameSpecific;
                renderer.PdfDocument.Save(PdfFile);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        #endregion FILLING METHODS PDF
    }
}