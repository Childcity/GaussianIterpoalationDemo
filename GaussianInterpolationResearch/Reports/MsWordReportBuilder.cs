using DataInterpolation;
using Interpolation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word;

namespace GaussianInterpolationResearch.Reports
{
	public struct ReportInputInfo
	{
		public FunctionInterpolation InterpolationData { get; set; }
		public Image GraphicImage { get; set; }
	}

	public class MsWordReportBuilder
	{
		private readonly ReportInputInfo[] inputInfo;

		public MsWordReportBuilder(ReportInputInfo[] reportInputInfo) => inputInfo = reportInputInfo;

		public async Task Build()
		{
			await startSTATask(() => buildReport());
		}

		private void buildReport()
		{

			object oMissing = System.Reflection.Missing.Value;
			object oEndOfDoc = "\\endofdoc"; /* \endofdoc is a predefined bookmark */

			//Start Word and create a new document.
			Word._Application oWord = new Word.Application();
			Word._Document oDoc = oWord.Documents.Add(ref oMissing, ref oMissing, ref oMissing, ref oMissing);
			//oDoc.tit = "Gorodetskiy_Interpolation_Report.docx";
			oDoc.PageSetup.TopMargin = 5;
			oDoc.PageSetup.BottomMargin = 5;

			try {
				inputInfo.ToList().ForEach(fucData => {
					var testFunction = fucData.InterpolationData.TestFunction;
					var correctPoints = fucData.InterpolationData.GetBasisAndFuncValues()[0].CorrectFuncValuesPoints;

					// Insert title
					Word.Paragraph oPara;
					object oRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
					oPara = oDoc.Content.Paragraphs.Add(ref oRng);
					oPara.Range.Text = $"Інтерполяція для Y = {testFunction.Name}";
					oPara.Range.Bold = 1;
					oPara.Range.Font.Size = 18;
					oPara.Range.InsertParagraphAfter();

					// Insert Interval
					oRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
					oPara = oDoc.Content.Paragraphs.Add(ref oRng);
					oPara.Range.Text = $"X є [{testFunction.XMin};{testFunction.XMax:F2}]. К-сть точок = {correctPoints.Count}";
					oPara.Range.Bold = 0;
					oPara.Range.Font.Size = 14;
					oPara.Range.InsertParagraphAfter();

					string stepForEachBasisPoint = string.Empty;
					for (int i = 1; i <= correctPoints.Count; i++) {
						stepForEachBasisPoint += $"{i} = {testFunction.GetStep(i):F2}; ";
					}
					oRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
					oPara = oDoc.Content.Paragraphs.Add(ref oRng);
					oPara.Range.Text = $"Крок = [{stepForEachBasisPoint}]";
					oPara.Range.Bold = 0;
					oPara.Range.Font.Size = 10;
					oPara.Format.SpaceAfter = 6;
					oPara.Range.InsertParagraphAfter();

					oRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
					oPara = oDoc.Content.Paragraphs.Add(ref oRng);
					oPara.Range.Text = $"Оцінка алгоритма:";
					oPara.Range.Bold = 1;
					oPara.Range.Font.Size = 14;
					oPara.Range.InsertParagraphAfter();

					var methodMarks = new Dictionary<Method, (double mark, List<double> distances)>();
					foreach (var methodInterpolatedPoints in fucData.InterpolationData.BuildInterpolations()) {
						BasisAndCorrectFuncValues basisAndFuncValues = fucData.InterpolationData.GetBasisAndFuncValues()[0];
						var distances = Halper.GetDistanceBetweenMethodPoints(basisAndFuncValues, methodInterpolatedPoints.Value);
						var methodMark = Halper.GetAlgorithmScore(distances);
						methodMarks[methodInterpolatedPoints.Key] = (methodMark, distances);
					}

					fucData.InterpolationData.GetInterpolationMethods()[0].ToList().ForEach(methodKV => {
						// Insert Score
						oRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
						oPara = oDoc.Content.Paragraphs.Add(ref oRng);
						oPara.Range.Text = $"{methodKV.Value.Name}" +
											$"\t\t\t{(methodKV.Key == Method.Lagrange ? "\t\t\t" : "")}" +
											$"{(methodKV.Key == Method.Gaus ? "\t" : "")}" +
											$"{methodMarks[methodKV.Key].mark:F16}";
						oPara.Range.Bold = 0;
						oPara.Range.Font.Size = 14;
						oPara.Range.InsertParagraphAfter();
					});

					// Insert Chart Image

					oRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
					oPara = oDoc.Content.Paragraphs.Add(ref oRng);
					Clipboard.Clear();
					Clipboard.SetImage(fucData.GraphicImage);
					oPara.Range.Paste();
					oPara.Range.InsertParagraphAfter();

					// Insert Space
					object oCollapseEnd = Word.WdCollapseDirection.wdCollapseEnd;
					object oPageBreak = Word.WdBreakType.wdPageBreak;
					oRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
					oPara = oDoc.Content.Paragraphs.Add(ref oRng);
					((Word.Range)oRng).Collapse(ref oCollapseEnd);
					((Word.Range)oRng).InsertBreak(ref oPageBreak);
					((Word.Range)oRng).Collapse(ref oCollapseEnd);
					oPara.Format.SpaceAfter = 0;
					//oPara.Range.InsertParagraphAfter();
				});

				oWord.Visible = true;
			} catch {
				oWord.Visible = true;
				throw;
			}
		}

		private static Bitmap resizeImage(Image image, int width, int height)
		{
			var destRect = new Rectangle(0, 0, width, height);
			var destImage = new Bitmap(width, height);

			destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

			using (var graphics = Graphics.FromImage(destImage)) {
				graphics.CompositingMode = CompositingMode.SourceCopy;
				graphics.CompositingQuality = CompositingQuality.HighQuality;
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

				using (var wrapMode = new ImageAttributes()) {
					wrapMode.SetWrapMode(WrapMode.TileFlipXY);
					graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
				}
			}

			return destImage;
		}

		private Task startSTATask(Action action)
		{
			TaskCompletionSource<object> source = new TaskCompletionSource<object>();
			Thread thread = new Thread(() => {
				try {
					action();
					source.SetResult(null);
				} catch (Exception ex) {
					source.SetException(ex);
				}
			});
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
			return source.Task;
		}
	}
}
