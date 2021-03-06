﻿using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using ClassLibrary;

namespace NozzleDisplay
{
    /// <summary>
    /// Lógica de interacción para SaveFile.xaml
    /// </summary>
    public partial class SaveFile : Window
    {
        double[] parametros;
        Nozzle nozzle;
        List<double> listdt;
        List<double> listdens;
        List<double> listtemp;
        List<double> listvel;
        List<double> listpres;

        public SaveFile()
        {
            InitializeComponent();
        }

        private void click_savefile(object sender, RoutedEventArgs e) // botón SAVE FILE
        {
            if (textbutton_fichero.Text == "") // si no se ha escrito un nombre
                MessageBox.Show("It is mandatory giving a name for the file.");
            else // si si se ha escrito
            {
                try
                {
                    //guardamos panel 
                    nozzle.GuardarEstadoFichero(textbutton_fichero.Text, this.parametros[0], this.parametros[1], this.parametros[2], Convert.ToInt32(this.parametros[3]), this.parametros[4], this.listdt, this.listdens, this.listtemp, this.listvel, this.listpres);

                    //cerramos form
                    this.Close();
                }
                catch
                {
                    MessageBox.Show("Something went wrong. Try again.");
                }
            }
        }
        public void getParametros(double[] param, Nozzle nozzle, List<double> listdt, List<double> listdens, List<double> listtemp, List<double> listvel, List<double> listpres) //coge los atributos que necesita
        {
            this.parametros = param;
            this.nozzle = nozzle;
            this.listdt = listdt;
            this.listdens = listdens;
            this.listtemp = listtemp;
            this.listvel = listvel;
            this.listpres = listpres;
        }
    }
}
