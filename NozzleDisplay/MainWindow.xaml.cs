﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ClassLibrary;

namespace NozzleDisplay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Nozzle nozzle;
        Rectangle[] nozzlerectangles;
        DataTable datanozzle;
        double dx;
        double dt;
        double C;
        int numR;
        List<double> listtemp, listdx, listvel, listpre, listden;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void fillCanvasNozzle()
        {
            for (int i = 0; i < nozzlerectangles.Length; i++)
            {
                Rectangulo rect_nozzle = this.nozzle.GetRectangulo(i);
                Rectangle rect_canvas = new Rectangle();
                rect_canvas.Height = Math.Min(rect_nozzle.GetAltura()*100,canvasNozzle.Height);
                rect_canvas.Width = canvasNozzle.Width / this.nozzle.GetNumRects();
                rect_canvas.Fill = new SolidColorBrush(Colors.White);
                rect_canvas.StrokeThickness = 0.1;
                rect_canvas.Stroke = Brushes.Black;
                canvasNozzle.Children.Add(rect_canvas);
                Canvas.SetLeft(rect_canvas, i * rect_canvas.Width);
                Canvas.SetTop(rect_canvas, (canvasNozzle.Height / 2) - (rect_canvas.Height / 2));
                nozzlerectangles[i] = rect_canvas;
            }
        }

        public Color GetColorMach(double rangeStart, double rangeEnd, double actualValue) // escala de color de la velocidad (Mach)
        {
            if (rangeStart >= rangeEnd) return Colors.Black;

            double max = rangeEnd - rangeStart; // make the scale start from 0
            double value = actualValue - rangeStart; // adjust the value accordingly

            double red = (255 * value) / max; 
            double blue = 255 - red; 

            return Color.FromRgb((Byte)red, (Byte)0, (Byte)blue);
        }

        public Color GetColorTemp(double rangeStart, double rangeEnd, double actualValue) // escala de color de la temperatura
        {

            if (actualValue >= rangeEnd) return Color.FromRgb((Byte)255, (Byte)0, (Byte)0);

            double max = rangeEnd - rangeStart; // make the scale start from 0
            double value = actualValue - rangeStart; // adjust the value accordingly

            double red = (255 * value) / max; 
            double blue = 255 - red;
            double green = blue / 2;

            return Color.FromRgb((Byte)red, (Byte)green, (Byte)blue);
        }

        public Color GetColorPressure(double rangeStart, double rangeEnd, double actualValue) // escala de color de la presión
        {
            if (rangeStart >= rangeEnd) return Colors.Black;

            double max = rangeEnd - rangeStart; // make the scale start from 0
            double value = actualValue - rangeStart; // adjust the value accordingly

            double blue = (255 * value) / max;
            double red = 255 - blue;
            double green = blue / 2;

            return Color.FromRgb((Byte)red, (Byte)green, (Byte)blue);
        }

        public Color GetColorDensity(double rangeStart, double rangeEnd, double actualValue) // escala de color de la densidad
        {
            if (actualValue >= rangeEnd) return Colors.Black;

            double max = rangeEnd - rangeStart; // make the scale start from 0
            double value = actualValue - rangeStart; // adjust the value accordingly

            double blue = 255 - ((255 * value) / max); 
            double red = Math.Min(255,blue*2*value);
            double green = 0;

            return Color.FromRgb((Byte)red, (Byte)green, (Byte)blue);
        }

        private void parambut_Click(object sender, RoutedEventArgs e) // botón SAVE
        {
            try
            {
                canvasNozzle.Children.Clear();

                this.C = Convert.ToDouble(cbox.Text);
                this.dx = Convert.ToDouble(dxbox.Text);
                this.numR = Convert.ToInt32(numrectbox.Text);

                this.nozzle = new Nozzle(this.numR, this.dx);
                this.dt = this.nozzle.getdt(this.C, this.dx);

                nozzlerectangles = new Rectangle[this.nozzle.GetNumRects()];

                fillCanvasNozzle();
                refreshCanvas();
                updateParameterlist();
                crearDataTable();
            }
            catch
            {
                MessageBox.Show("Error");
            }

            //// comprobamos que no estén vacíos
            //if (dxbox.Text == "" || cbox.Text == "" || cbox_Copy.Text == "")
            //    MessageBox.Show("All parameters should be established\n(^t, ^x & number of rectangles)");
            //else
            //{
            //    // comprobamos que tengan el formato que han de tener
            //    try
            //    {
            //        // comprobamos que sean positivos y diferentes de 0
            //        double dt = Convert.ToDouble(dxbox.Text);
            //        double dx = Convert.ToDouble(cbox.Text);
            //        int numrect = Convert.ToInt32(cbox_Copy.Text);
            //        if (dt <= 0 || dx <= 0 || numrect <= 0)
            //            MessageBox.Show("All parameters should be positive and different from 0");
            //        else
            //        {
            //            // guardamos los parámetros
            //            this.dt = dt;
            //            this.dx = dx;
            //            this.numR = numrect;
            //        }
            //    }
            //    catch 
            //    {
            //        MessageBox.Show("The parameters ^t and ^x can be decimal numbers, but the number of rectangles not\nPlease check it");
            //    }
            //}
        }

        private void comboboxcolor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                refreshCanvas();
            }
            catch
            {

            }
        }

        private void refreshCanvas() // actualiza la visualización (colores)
        {
            if (comboboxcolor.SelectedIndex == 0) //Pressure
            {
                for (int i = 0; i < nozzlerectangles.Length; i++)
                {
                    Rectangle rect_canvas = this.nozzlerectangles[i];
                    Rectangulo rect_nozzle = this.nozzle.GetRectangulo(i);
                    rect_canvas.Fill = new SolidColorBrush(GetColorPressure(0, 1, rect_nozzle.GetPresP()));
                }

                LinearGradientBrush lgb = new LinearGradientBrush(GetColorPressure(0, 1, 1), GetColorPressure(0, 1, 0), 90);
                GradientStop gs = new GradientStop();
                gs.Color = GetColorPressure(0, 1, 0.5);
                gs.Offset = 0.5;
                lgb.GradientStops.Add(gs);
                rectanglescale.Fill = lgb;
            }

            if (comboboxcolor.SelectedIndex == 1) //Velocity
            {
                for (int i = 0; i < nozzlerectangles.Length; i++)
                {
                    Rectangle rect_canvas = this.nozzlerectangles[i];
                    Rectangulo rect_nozzle = this.nozzle.GetRectangulo(i);
                    rect_canvas.Fill = new SolidColorBrush(GetColorMach(0, 4, rect_nozzle.GetVelP()));
                }

                LinearGradientBrush lgb = new LinearGradientBrush(GetColorMach(0, 2.5, 2.5), GetColorMach(0, 2.5, 0), 90);
                GradientStop gs = new GradientStop();
                gs.Color = GetColorMach(0, 2.5, 1.25);
                gs.Offset = 0.5;
                lgb.GradientStops.Add(gs);
                rectanglescale.Fill = lgb;
            }

            if (comboboxcolor.SelectedIndex == 2) //Temperature
            {
                for (int i = 0; i < nozzlerectangles.Length; i++)
                {
                    Rectangle rect_canvas = this.nozzlerectangles[i];
                    Rectangulo rect_nozzle = this.nozzle.GetRectangulo(i);
                    rect_canvas.Fill = new SolidColorBrush(GetColorTemp(0, 1, rect_nozzle.GetTempP()));
                }

                LinearGradientBrush lgb = new LinearGradientBrush(GetColorTemp(0, 1, 1), GetColorTemp(0, 1, 0), 90);
                GradientStop gs = new GradientStop();
                gs.Color = GetColorTemp(0, 1, 0.5);
                gs.Offset = 0.5;
                lgb.GradientStops.Add(gs);
                rectanglescale.Fill = lgb;
            }

            if (comboboxcolor.SelectedIndex == 3) //Density
            {
                for (int i = 0; i < nozzlerectangles.Length; i++)
                {
                    Rectangle rect_canvas = this.nozzlerectangles[i];
                    Rectangulo rect_nozzle = this.nozzle.GetRectangulo(i);
                    rect_canvas.Fill = new SolidColorBrush(GetColorDensity(0, 1, rect_nozzle.GetDensP()));
                }

                LinearGradientBrush lgb = new LinearGradientBrush(GetColorDensity(0, 1, 1), GetColorDensity(0, 1, 0), 90);
                GradientStop gs = new GradientStop();
                gs.Color = GetColorDensity(0, 1, 0.5);
                gs.Offset = 0.5;
                lgb.GradientStops.Add(gs);
                rectanglescale.Fill = lgb;
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e) // botón ANDERSON
        {
            Anderson a = new Anderson();
            a.ShowDialog();
        }


        private void EjecutarUnCiclo() // función que ejecuta un ciclo
        {
            this.nozzle.EjecutarCiclo(this.dt, this.dx, 1.4);
            this.nozzle.ActualizarEstados();
            this.refreshCanvas();
            this.updateParameterlist();
            this.crearDataTable();
        }
        private void Click_Aboutus(object sender, RoutedEventArgs e)
        {
            Aboutus au = new Aboutus();
            au.ShowDialog();
        }

        private void onestepbut_Click(object sender, RoutedEventArgs e)
        {
            EjecutarUnCiclo();
        }

        private void crearDataTable()
        {
            datanozzle = new DataTable();

            datanozzle.Columns.Add(new DataColumn(" "));

            for (int i = 0; i < this.numR; i++)
            {
                datanozzle.Columns.Add(new DataColumn((i + 1).ToString()));
            }

            DataRow dr_dx = datanozzle.NewRow(); dr_dx[0] = ("X L");
            DataRow dr_p = datanozzle.NewRow(); dr_p[0] = ("P Po");
            DataRow dr_v = datanozzle.NewRow(); dr_v[0] = ("V Vo");
            DataRow dr_t = datanozzle.NewRow(); dr_t[0] = ("T To");
            DataRow dr_de = datanozzle.NewRow(); dr_de[0] = ("p po");

            for (int i = 1; i < datanozzle.Columns.Count; i++)
            {
                dr_dx[i] = listdx[i - 1];
                dr_p[i] = listpre[i - 1];
                dr_v[i] = listvel[i - 1];
                dr_t[i] = listtemp[i - 1];
                dr_de[i] = listden[i - 1];
            }

            datanozzle.Rows.Add(dr_dx);
            datanozzle.Rows.Add(dr_p);
            datanozzle.Rows.Add(dr_v);
            datanozzle.Rows.Add(dr_t);
            datanozzle.Rows.Add(dr_de);


            gridnozzle.ItemsSource = datanozzle.DefaultView;
            gridnozzle.DataContext = datanozzle.DefaultView;
            gridnozzle.Items.Refresh();
        }

        private void updateParameterlist()
        {
            this.listdx = this.nozzle.getNozzleXL(this.dx);
            this.listpre = this.nozzle.getPressures();
            this.listvel = this.nozzle.getVelocities();
            this.listtemp = this.nozzle.getTemperatures();
            this.listden = this.nozzle.getDensities();
        }
    }
}
