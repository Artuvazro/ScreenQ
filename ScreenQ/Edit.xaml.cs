using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ScreenQ
{
    /// <summary>
    /// Lógica de interacción para Edit.xaml
    /// </summary>
    public partial class Edit : Window
    {
        public Edit()
        {
            InitializeComponent();
            capturedImage.Source = MainWindow.capture;
            paintSurface.Width = MainWindow.capture.Width;
            paintSurface.Height = MainWindow.capture.Height;
            BrushColor = Color.FromArgb(0xff, 0xff, 0x00, 0x00);
            myBrush = new SolidColorBrush(BrushColor);
            LoadLastButton();
            ReadTypology readTypoClass = new ReadTypology();

            SizeBox.Text = Properties.Settings.Default["DefaultToolSize"].ToString();

            if (Properties.Settings.Default["XMLTypoPath"].ToString() != "")
            {
                try
                {
                    ErrorListMenu = readTypoClass.ReadTypologyFromXml(Properties.Settings.Default["XMLTypoPath"].ToString(), this);
                    return;
                }
                catch
                {
                    MessageBox.Show("Your error typology could not be loaded. Please, check your file and see if the XML format is correct.", "Error loading typology", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
                PopulateErrorContextMenu();              
        }

        private Point currentPoint = new Point();
        private Point startPoint;
        private Rectangle rect;
        private Ellipse circle;
        private Line line;
        private static Color BrushColor;
        private SolidColorBrush myBrush = new SolidColorBrush();
        private enum Tool { MANUAL, RECTANGLE, CIRCLE, ERASER };
        Tool SelectedTool = (Tool)Enum.Parse(typeof(Tool), Properties.Settings.Default["SavedLastTool"].ToString());
        private int strokeSize = 5;
        private int lineNumber;
        private int rectangleNumber;
        private int circleNumber;
        private ContextMenu ErrorListMenu = new ContextMenu();
        private bool drawingNow = false;
        private Button lastButton;
        private String selectedError;
        private List<String> selectedErrors = new List<String>();
        private List<String> selectedErrorBranch = new List<String>();


        private void PopulateErrorContextMenu()
        {
            MenuItem mi = new MenuItem()
            {
                Header = "(No error typology loaded)"
            };
            ErrorListMenu.Items.Add(mi);
        }

        private void LoadLastButton()
        {
            switch (Properties.Settings.Default["SavedLastTool"].ToString())
            {
                default:
                case "MANUAL":
                    Cursor = Cursors.Pen;
                    PencilButton.Background = Brushes.LightSkyBlue;
                    PencilButton.BorderBrush = Brushes.CornflowerBlue;
                    lastButton = PencilButton;
                    break;
                case "RECTANGLE":
                    Cursor = Cursors.Cross;
                    RectangleButton.Background = Brushes.LightSkyBlue;
                    RectangleButton.BorderBrush = Brushes.CornflowerBlue;
                    lastButton = RectangleButton;
                    break;
                case "CIRCLE":
                    Cursor = Cursors.Cross;
                    CircleButton.Background = Brushes.LightSkyBlue;
                    CircleButton.BorderBrush = Brushes.CornflowerBlue;
                    lastButton = CircleButton;
                    break;
            }
        }

        private void ChangeButtonState(Button myButton)
        {
            myButton.Background = Brushes.LightSkyBlue;
            myButton.BorderBrush = Brushes.CornflowerBlue;

            lastButton.Background = Brushes.LightGray;
            lastButton.BorderBrush = Brushes.LightGray;

            lastButton = myButton;
        }

        private void SelectPencil_Button(object sender, RoutedEventArgs e)
        {
            SelectedTool = Tool.MANUAL;
            if (lastButton != PencilButton)
            {
                ChangeButtonState(PencilButton);
                Cursor = Cursors.Pen;
                Properties.Settings.Default["SavedLastTool"] = "MANUAL";
                Properties.Settings.Default.Save();
            }
        }

        private void SelectRectangle_Button(object sender, RoutedEventArgs e)
        {
            SelectedTool = Tool.RECTANGLE;
            if (lastButton != RectangleButton)
            {
                ChangeButtonState(RectangleButton);
                Cursor = Cursors.Cross;
                Properties.Settings.Default["SavedLastTool"] = "RECTANGLE";
                Properties.Settings.Default.Save();
            }
                
        }

        private void SelectCircle_Button(object sender, RoutedEventArgs e)
        {
            SelectedTool = Tool.CIRCLE;
            if (lastButton != CircleButton)
            {
                ChangeButtonState(CircleButton);
                Cursor = Cursors.Cross;
                Properties.Settings.Default["SavedLastTool"] = "CIRCLE";
                Properties.Settings.Default.Save();
            }
        }

        private void SelectEraser_Button(object sender, RoutedEventArgs e)
        {
            SelectedTool = Tool.ERASER;
            if (lastButton != EraserButton)
            {
                ChangeButtonState(EraserButton);
                Cursor = Cursors.Arrow;
            }
                
        }

        private void Canvas_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            switch (SelectedTool)
            {
                case Tool.MANUAL:
                    if (e.ButtonState == MouseButtonState.Pressed)
                        currentPoint = e.GetPosition(paintSurface);                    
                    break;

                case Tool.RECTANGLE:
                    startPoint = e.GetPosition(paintSurface);

                    rect = new Rectangle
                    {
                        Stroke = myBrush,
                        StrokeThickness = strokeSize
                    };

                    Canvas.SetLeft(rect, startPoint.X);
                    Canvas.SetTop(rect, startPoint.X);

                    paintSurface.Children.Add(rect);
                    break;

                case Tool.CIRCLE:
                    startPoint = e.GetPosition(paintSurface);

                    circle = new Ellipse
                    {
                        Stroke = myBrush,
                        StrokeThickness = strokeSize,
                    };
                    Canvas.SetLeft(circle, startPoint.X);
                    Canvas.SetTop(circle, startPoint.X);
                    paintSurface.Children.Add(circle);
                    break;

                case Tool.ERASER:
                    Point pt = e.GetPosition(paintSurface);
                    HitTestResult result = VisualTreeHelper.HitTest(paintSurface, pt);

                    if ((result != null) && ((result.VisualHit is Image) == false))
                    {
                        var name = result.VisualHit.ReadLocalValue(NameProperty);
                        var label = (from c in paintSurface.Children.OfType<Label>() where name.Equals(c.Name) select c).DefaultIfEmpty(null).Single();
                        paintSurface.Children.Remove(result.VisualHit as UIElement);
                        paintSurface.Children.Remove(label);

                        if (result.VisualHit is Line)
                        {
                            var lines = (from c in paintSurface.Children.OfType<Line>() where name.Equals(c.Name) select c).ToArray();
                            foreach (var myLine in lines)
                            {
                                paintSurface.Children.Remove(myLine);                                
                            }
                        }

                    }
                    break;
            }
        }
        
        private void Canvas_MouseMove_1(object sender, MouseEventArgs e)
        {
            switch (SelectedTool)
            {
                case Tool.MANUAL:
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        line = new Line()
                        {
                            Stroke = myBrush,
                            X1 = currentPoint.X,
                            Y1 = currentPoint.Y,
                            X2 = e.GetPosition(paintSurface).X,
                            Y2 = e.GetPosition(paintSurface).Y,
                            StrokeThickness = strokeSize,
                            StrokeStartLineCap = PenLineCap.Round,
                            StrokeEndLineCap = PenLineCap.Round,
                            StrokeDashCap = PenLineCap.Round,
                            StrokeLineJoin = PenLineJoin.Round,
                            Name = "myline" + lineNumber
                        };
                        currentPoint = e.GetPosition(paintSurface);
                        if (ErrorListMenu.IsVisible == true)
                            drawingNow = false;
                        else
                        {
                            drawingNow = true;
                            paintSurface.Children.Add(line);
                        }
                    }
                    break;

                case Tool.RECTANGLE:
                    if (e.LeftButton == MouseButtonState.Released || rect == null)
                        return;

                    var pos = e.GetPosition(paintSurface);

                    var rx = Math.Min(pos.X, startPoint.X);
                    var ry = Math.Min(pos.Y, startPoint.Y);

                    var rw = Math.Max(pos.X, startPoint.X) - rx;
                    var rh = Math.Max(pos.Y, startPoint.Y) - ry;
                    
                    rect.Width = rw;
                    rect.Height = rh;
                    rect.Name = "myrectangle" + rectangleNumber;

                    Canvas.SetLeft(rect, rx);
                    Canvas.SetTop(rect, ry);
                    drawingNow = true;

                    break;

                case Tool.CIRCLE:
                    if (e.LeftButton == MouseButtonState.Released || circle == null)
                        return;
                    
                    var cpos = e.GetPosition(paintSurface);

                    var cx = Math.Min(cpos.X, startPoint.X);
                    var cy = Math.Min(cpos.Y, startPoint.Y);

                    var cw = Math.Max(cpos.X, startPoint.X) - cx;
                    var ch = Math.Max(cpos.Y, startPoint.Y) - cy;

                    circle.Width = cw;
                    circle.Height = ch;
                    circle.Name = "mycircle" + circleNumber;

                    Canvas.SetLeft(circle, cx);
                    Canvas.SetTop(circle, cy);
                    drawingNow = true;
                    break;
            }
        }

        private double UpPosX;
        private double UpPosY;

        private void Canvas_MouseUp_1(object sender, MouseButtonEventArgs e)
        {
            UpPosX = Mouse.GetPosition(paintSurface).X;
            UpPosY = Mouse.GetPosition(paintSurface).Y;

            switch (SelectedTool)
            {
                case Tool.MANUAL:
                    if (drawingNow == true)
                    {
                        lineNumber++;
                        line = null;
                        OpenErrorList();
                    }
                    break;

                case Tool.RECTANGLE:
                    if ((rect.ActualHeight <= strokeSize) && rect.ActualWidth <= strokeSize)
                    {
                        paintSurface.Children.Remove(rect);
                        drawingNow = false;
                    }
                    if (drawingNow == true)
                    {
                        rectangleNumber++;
                        rect = null;
                        drawingNow = false;
                        OpenErrorList();
                    }
                    break;

                case Tool.CIRCLE:
                    if ((circle.ActualHeight <= strokeSize) && circle.ActualWidth <= strokeSize)
                    {
                        paintSurface.Children.Remove(circle);
                        drawingNow = false;
                    }
                    if(drawingNow == true)
                    {
                        circleNumber++;
                        circle = null;
                        OpenErrorList();
                    }
                    break;

                case Tool.ERASER:
                    break;
            }
        }

        private void ColorPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (ColorPicker.SelectedIndex)
            {
                case 0:
                    BrushColor = Color.FromArgb(0xff, 0xff, 0x00, 0x00);
                    myBrush = new SolidColorBrush(BrushColor);
                    break;
                case 1:
                    BrushColor = Color.FromArgb(0xff, 0x00, 0x00, 0xff);
                    myBrush = new SolidColorBrush(BrushColor);
                    break;
                case 2:
                    BrushColor = Color.FromArgb(0xff, 0xff, 0xd7, 0x00);
                    myBrush = new SolidColorBrush(BrushColor);
                    break;
                case 3:
                    BrushColor = Color.FromArgb(0xff, 0x5a, 0xcd, 0x32);
                    myBrush = new SolidColorBrush(BrushColor);
                    break;
                case 4:
                    BrushColor = Color.FromArgb(0xff, 0x00, 0x00, 0x00);
                    myBrush = new SolidColorBrush(BrushColor);
                    break;
                case 5:
                    BrushColor = Color.FromArgb(0xff, 0xff, 0xff, 0xff);
                    myBrush = new SolidColorBrush(BrushColor);
                    break;
            }
        }

        private string previousSize;

        private void SizeBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var SizeOnlyNumbers = Regex.Replace(SizeBox.Text, "[^0-9]", "");
            int output;

            if (Int32.TryParse(SizeOnlyNumbers, out output))
            {
                strokeSize = output;
                Properties.Settings.Default["DefaultToolSize"] = output + " px";
                Properties.Settings.Default.Save();
            }

            else
                SizeBox.Text = previousSize;
        }

        private void SizeBox_GotFocus(object sender, RoutedEventArgs e)
        {
            previousSize = SizeBox.Text;
        }

        private void SizeBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if(!SizeBox.Text.Contains("px"))
            SizeBox.Text += " px";
        }

        private void OpenErrorList()
        {
            ErrorListMenu.IsOpen = true;
        }

        public void Item_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;

            if (!mi.HasItems)
            { //Si quitas el if, te devuelve todas las categorías de la rama, eso es útil para imprimir en el Excel.
                selectedErrorBranch.Add("\r");
                selectedError = mi.Header.ToString();
                selectedErrors.Add(selectedError);

                Label ErrorLabel = new Label()
                {
                    Content = selectedError,
                    FontSize = 12,
                    FontWeight = FontWeights.Bold,
                    Background = myBrush,
                    Foreground = Brushes.White,
                };
                if (SelectedTool == Tool.MANUAL)
                    ErrorLabel.Name = "myline" + (lineNumber - 1);

                if(SelectedTool == Tool.RECTANGLE)
                    ErrorLabel.Name = "myrectangle" + (rectangleNumber - 1);

                if(SelectedTool == Tool.CIRCLE)
                    ErrorLabel.Name = "mycircle" + (circleNumber - 1);

                Canvas.SetLeft(ErrorLabel, UpPosX);
                Canvas.SetTop(ErrorLabel, UpPosY);
                paintSurface.Children.Add(ErrorLabel);
            }
            else
            {
                selectedErrorBranch.Add(" > ");
            }
            selectedErrorBranch.Add(mi.Header.ToString());
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
            (int)paintSurface.Width, (int)paintSurface.Height, 96d, 96d, PixelFormats.Pbgra32);
            paintSurface.Measure(new Size((int)paintSurface.Width, (int)paintSurface.Height));
            paintSurface.Arrange(new Rect(new Size((int)paintSurface.Width, (int)paintSurface.Height)));
            renderBitmap.Render(paintSurface);
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

            int screenID = (int)Properties.Settings.Default["ScreenID"];

            using (FileStream file = File.Create(Properties.Settings.Default["ScreenShotSavePath"].ToString() + "\\screenshot - " + screenID + ".png"))
            {
                encoder.Save(file);
                screenID++;
                Properties.Settings.Default["ScreenID"] = screenID;
                Properties.Settings.Default.Save();
            }
            if(File.Exists(Properties.Settings.Default["ScreenShotSavePath"].ToString() + "\\ScreenQ - Report.xlsx"))
                Excel.WriteExcel(encoder, selectedErrors, selectedErrorBranch, additionalBox.Text);
            else
            {
                Excel.CreateExcel();
                Excel.WriteExcel(encoder, selectedErrors, selectedErrorBranch, additionalBox.Text);
            }
                

            if(Convert.ToBoolean(Properties.Settings.Default["SaveAndClose"]) == true)
            {
                this.Close();
            }
        }
    }
}
