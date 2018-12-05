using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
//using System.Windows.Forms;

using AutoItX3Lib;
using System.Windows.Threading;
using System.Globalization;

namespace VIVE
{
    public partial class MainWindow : Window
    {
        // AutoIt
        AutoItX3 AutoIt;
        DispatcherTimer Timer_AutoIt;
        string AutoItTarget = "VIVE";

        //Timer
        DispatcherTimer Timer;
        TimeSpan time;
        int tiempoCargado = 0;
        bool tiempoCargaFinalizado = false;

        //Games
        List<string[]> games = new List<string[]>();
        string[] gameSelected;

        //Egresos
        string tipoEgreso = "Compra";

        //Reporte y configuracion
        Reporte1 reporte = new Reporte1();
        Configuracion configuracion = new Configuracion();
        bool IsCortesia = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Inicializa AutoIt
            AutoIt = new AutoItX3();

            //Generar y gargar Reportes y precios
            reporte.GenerateReport();

            //Cargar configuracion
            configuracion.LoadConfig();

            //Cargar juegos
            games = configuracion.GetGames();

            //Inicializar el tiempo de timer
            time = TimeSpan.FromMinutes(0);
            txbTimer.Text = time.ToString(@"m\:ss");

            //Inicializar el timer de juegos
            Timer = new DispatcherTimer(System.Windows.Threading.DispatcherPriority.Normal);
            Timer.Tick += new EventHandler(Timer_);
            Timer.Interval = new TimeSpan(0, 0, 0, 1);

            //iniciaizar el timer de AutoIt
            Timer_AutoIt = new DispatcherTimer();
            Timer_AutoIt.Tick += new EventHandler(Timer_AutoIt_);
            Timer_AutoIt.Interval = new TimeSpan(0, 0, 0, 2);
            Timer_AutoIt.Start();

            //Cargar Primer menu
            ShowReproduccionesMenu(sender, e);

            //Abrir SteamVR  
            if (AutoIt.WinExists("SteamVR", "") == 0)
            {
                int PID = AutoIt.Run(Environment.GetEnvironmentVariable("ProgramFiles(x86)") + "/Steam/steamapps/common/SteamVR/bin/win64/vrmonitor.exe", "", AutoIt.SW_HIDE);
                if (PID != 0) { AutoIt.WinWaitActive("SteamVR"); }
                else { MessageBoxResult messBox = MessageBox.Show("El SteamVR no se pudo abrir", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Error); }
            }
        }

        #region Caja functions
        private void lstMenuCaja_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (lstMenuCaja.SelectedIndex)
            {
                case 0: ShowAperturaMenu(); break;
                case 1: ShowCierreMenu(); break;
                case 2: ShowEgresoMenu(); break;
                case 3: ShowArqueoMenu(); break;
            }
        }
        #endregion

        #region Apertura functions
        private void txtIDOperario_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            OpenNumericPad();
            txtIDOperario.Text = "";
        }

        private void txtIDOperario_LostFocus(object sender, RoutedEventArgs e)
        {
            CloseNumericPad();
            if(txtIDOperario.Text == "") txtIDOperario.Text = "Ingrese su ID";
        }

        private void txtIDOperario_KeyDown(object sender, KeyEventArgs e)
        {
            CloseNumericPadWithEnter(e);
        }

        private void txtEfectivoBaseOperario_LostFocus(object sender, RoutedEventArgs e)
        {
            CloseNumericPad();
            if (txtEfectivoBaseOperario.Text == "") txtEfectivoBaseOperario.Text = "Ingrese el efectivo base";
        }

        private void txtEfectivoBaseOperario_KeyDown(object sender, KeyEventArgs e)
        {
            CloseNumericPadWithEnter(e);
        }

        private void txtEfectivoBaseOperario_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            OpenNumericPad();
            txtEfectivoBaseOperario.Text = "";
        }

        private void btnRegistrarApertura_Click(object sender, RoutedEventArgs e)
        {
            string baseEfectivo = txtEfectivoBaseOperario.Text;
            string id = txtIDOperario.Text;
            if (configuracion.OpenCaja(id, baseEfectivo))
            {
                //Incremente Reporte de apertura
                reporte.IncreaseAperturaReport(id, baseEfectivo);

                MessageBoxResult messBox = MessageBox.Show("La apertura de la caja fue hecha satisfactoriamente", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
        }
        #endregion

        #region Cierre functions
        private void txtEfectivoJornadaOperario_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            OpenNumericPad();
            txtEfectivoJornadaOperario.Text = "";
        }

        private void txtEfectivoJornadaOperario_KeyDown(object sender, KeyEventArgs e)
        {
            CloseNumericPadWithEnter(e);
        }

        private void txtEfectivoJornadaOperario_LostFocus(object sender, RoutedEventArgs e)
        {
            CloseNumericPad();
            if (txtEfectivoJornadaOperario.Text == "") txtEfectivoJornadaOperario.Text = "Ingrese el efectivo";
        }

        private void btnRegistrarCierre_Click(object sender, RoutedEventArgs e)
        {
            string efectivo = txtEfectivoJornadaOperario.Text;
            if (configuracion.CloseCaja(efectivo))
            {
                //Incremente Reporte de apertura
                reporte.IncreaseCierreReport(efectivo);

                MessageBoxResult messBox = MessageBox.Show("El cierre de la caja fue hecho satisfactoriamente", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        #endregion

        #region Egresos functions
        private void txtIDReceptorEgreso_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            OpenNumericPad();
            txtIDReceptorEgreso.Text = "";
        }

        private void txtIDReceptorEgreso_KeyDown(object sender, KeyEventArgs e)
        {
            CloseNumericPadWithEnter(e);
        }

        private void txtIDReceptorEgreso_LostFocus(object sender, RoutedEventArgs e)
        {
            CloseNumericPad();
            if (txtIDReceptorEgreso.Text == "") txtIDReceptorEgreso.Text = "Ingrese el ID";
        }

        private void txtValorEgreso_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            OpenNumericPad();
            txtValorEgreso.Text = "";
        }

        private void txtValorEgreso_KeyDown(object sender, KeyEventArgs e)
        {
            CloseNumericPadWithEnter(e);
        }

        private void txtValorEgreso_LostFocus(object sender, RoutedEventArgs e)
        {
            CloseNumericPad();
            if (txtValorEgreso.Text == "") txtValorEgreso.Text = "Ingrese el valor";
        }

        private void lwcboTipoEgreso_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (lwcboTipoEgreso.SelectedIndex)
            {
                case 0: tipoEgreso = "Cambio"; break;
                case 1: tipoEgreso = "Gasto"; break;
                case 2: tipoEgreso = "Entrega de dinero"; break;
                case 3: tipoEgreso = "Cambio de caja"; break;
                case 4: tipoEgreso = "Consignación"; break;
            }
        }

        private void btnRegistarEgreso_Click(object sender, RoutedEventArgs e)
        {
            string idReceptor = txtIDReceptorEgreso.Text;
            string valorEgreso = txtValorEgreso.Text;

            if (configuracion.RegisterEgress(idReceptor,valorEgreso))
            {
                //Incremente Reporte de cierre
                reporte.IncreaseEgresoReport(idReceptor, valorEgreso, tipoEgreso);

                MessageBoxResult messBox = MessageBox.Show("Se ha registrado el egreso satisfactoriamente", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }
        #endregion

        #region Reproduccion functions

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (configuracion.IsCajaOpen())
            {
                if (!Timer.IsEnabled)
                {
                    if (Int32.TryParse(txtTiempo.Text, out tiempoCargado))
                    {
                        if (tiempoCargado != 0)
                        {
                            //Verificar que el juego existe y guardarlo si es el caso
                            string gameid = txtJuegos.Text.ToString();
                            if (GameExists(gameid))
                            {
                                //Guardar Juego Seleccionado
                                gameSelected = GetGameSelected(gameid);
                            }
                            else  //Si no existe se informa 
                            {
                                MessageBoxResult messBox = MessageBox.Show("El juego con el Id introducido no esta en la lista de juegos", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return;
                            }

                            //Guardar si es cortesia
                            if(chkCortesia.IsChecked.HasValue && chkCortesia.IsChecked.Value)
                            {
                                IsCortesia = true;
                            }

                            //Correr juego con AutoIt
                            AutoItTarget = gameSelected[2];
                            int PID = AutoIt.Run(gameSelected[1], "", 1);
                            if (PID != 0)
                            {
                                //Esperar a que la pantalla este activa
                                if (AutoIt.WinWaitActive(AutoItTarget, "", 15) == 0)
                                {
                                    AutoIt.WinKill(AutoItTarget);
                                    MessageBoxResult messBox = MessageBox.Show("El juego seleccionado no se abrió de forma correcta, el juego se cerrará a continuación", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                    return;
                                }  
                            }
                            else { MessageBoxResult messBox = MessageBox.Show("El juego seleccionado no existe o la ruta es incorrecta", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return; }

                            if (System.Windows.Forms.Screen.AllScreens.Count() > 1)
                                //Posicionar ventana en la pantalla segundaria
                                AutoIt.WinMove(AutoItTarget, "", (int)System.Windows.SystemParameters.PrimaryScreenWidth, 0);
                            //Manterner la ventana del juego de primera    
                            AutoIt.WinSetOnTop(AutoItTarget, "", 1);
                            //Mostrar pantalla                                                         
                            AutoIt.WinSetState(AutoItTarget, "", AutoIt.SW_SHOW);
                            //Maximizar pantalla                                            
                            AutoIt.WinSetState(AutoItTarget, "", AutoIt.SW_MAXIMIZE);                                          
                            
                            //Iniciar timer con tiempo de carga
                            try
                            {
                                time = TimeSpan.FromSeconds(int.Parse(gameSelected[3]));
                                txbTimer.Text = time.ToString(@"mm\:ss");
                                tiempoCargaFinalizado = false;
                                Timer.Start();
                            }
                            catch { MessageBoxResult messBox = MessageBox.Show("El tiempo de carga del juego no es un numero entero", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }

                            //Cambiar habilitacion de botones
                            btnStop.IsEnabled = true;
                            btnTeamViewer.IsEnabled = false;
                            btnPlay.IsEnabled = false;
                        }
                        else { MessageBoxResult messBox = MessageBox.Show("El tiempo de juego debe ser mayor a cero", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Exclamation);  }
                    }
                    else { MessageBoxResult messBox = MessageBox.Show("El tiempo cargado no es un numero entero", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                }
            }
            else {  MessageBoxResult messBox = MessageBox.Show("La caja no ha sido abierta", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Exclamation);  }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            if (Timer.IsEnabled)
            {
                if (tiempoCargaFinalizado)
                {
                    //Calcular tiempo jugado
                    int tiempoReporte = tiempoCargado - (int)Math.Floor(time.TotalMinutes);

                    //Incrementar reporte de reproduccion y Guardar venta
                    if (!IsCortesia)
                    {
                        reporte.IncreaseReproduccionesReport(gameSelected[0], tiempoReporte);
                        configuracion.SaveSale(tiempoReporte);

                        txbTotalVenta.Text = "El total de la venta es: $ " + configuracion.GetBaseRate() * tiempoReporte;
                    }
                    else
                    {
                        reporte.IncreaseCortesiasReport(gameSelected[0], tiempoReporte);
                        configuracion.RegisterCortesias();
                    }

                    
                }
                else
                {
                    //Registrar Error
                    configuracion.RegisterError();

                    //Incremente reporte de error
                    reporte.IncreaseErrorReport(gameSelected[0]);
                }
                //Cerrar en juego
                AutoIt.WinKill(AutoItTarget);
                AutoItTarget = "VIVE";

                //Reinicar el tiempo
                time = TimeSpan.FromMinutes(0);
                txbTimer.Text = time.ToString(@"mm\:ss");

                //Detener el timer
                Timer.Stop();

                //Quitar check al checkbox de cortesias
                chkCortesia.IsChecked = false;
                IsCortesia = false;

                //Cambiar habilitacion de botones
                btnAgregar.IsEnabled = false;
                btnStop.IsEnabled = false;
                btnTeamViewer.IsEnabled = true;
                btnPlay.IsEnabled = true;
            }
        }

        private void btnAgregar_Click(object sender, RoutedEventArgs e)
        {
            int tiempoAgregado;
            if (!Int32.TryParse(txtTiempo.Text, out tiempoAgregado))
            {
                MessageBoxResult messBox = MessageBox.Show("El tiempo cargado no es un numero entero", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            time = time.Add(TimeSpan.FromMinutes(tiempoAgregado));
        }

        private void txtTiempo_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            OpenNumericPad();
            txtTiempo.Text = "";
        }

        private void txtTiempo_KeyDown(object sender, KeyEventArgs e)
        {
            CloseNumericPadWithEnter(e);
        }

        private void txtTiempo_LostFocus(object sender, RoutedEventArgs e)
        {
            CloseNumericPad();
        }

        private void txtJuegos_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            OpenNumericPad();
            txtJuegos.Text = "";
        }

        private void txtJuegos_KeyDown(object sender, KeyEventArgs e)
        {
            CloseNumericPadWithEnter(e);
        }

        private void txtJuegos_LostFocus(object sender, RoutedEventArgs e)
        {
            CloseNumericPad();
        }

        private void btnTeamViewer_Click(object sender, RoutedEventArgs e)
        {
            if(btnTeamViewer.Content.ToString() == "Abrir TeamViewer")
            {
                //Cambiar contenido del boton
                btnTeamViewer.Content = "Cerrar TeamViewer";

                //Abrir teamviewer
                if (AutoIt.WinExists("Team Viewer", "") == 0)
                {
                    AutoItTarget = "TeamViewer";
                    int PID = AutoIt.Run(Environment.GetEnvironmentVariable("ProgramFiles(x86)") + "/TeamViewer/TeamViewer.exe", "", 1);
                    if (PID != 0) { AutoIt.WinWaitActive(AutoItTarget);  }    
                    else { MessageBoxResult messBox = MessageBox.Show("TeamViewer no se pudo abrir", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Error); }
                    if (System.Windows.Forms.Screen.AllScreens.Count() > 1)
                        AutoIt.WinMove(AutoItTarget, "", (int)System.Windows.SystemParameters.PrimaryScreenWidth - 7, 0);   //Posicionar ventana en la pantalla segundaria
                    AutoIt.WinSetOnTop(AutoItTarget, "", 1);                                                            //Manterner la ventana del juego de primera    
                    AutoIt.WinSetState(AutoItTarget, "", AutoIt.SW_SHOW);                                               //Mostrar pantalla
                    AutoIt.WinSetState(AutoItTarget, "", AutoIt.SW_MAXIMIZE);
                }
            }
            else
            {
                //Cambiar contenido del boton
                btnTeamViewer.Content = "Abrir TeamViewer";

                //Cerrar teamviewer
                while(AutoIt.WinExists("TeamViewer", "") == 1)
                {
                    AutoIt.WinClose("TeamViewer", "");
                    AutoIt.WinWaitClose("TeamViewer", "", 1);
                }
                AutoItTarget = "VIVE";
            }
        }

        #endregion

        #region Salir funtions
        private void pbPassword_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            OpenNumericPad();
            pbPassword.Clear();
        }

        private void pbPassword_KeyDown(object sender, KeyEventArgs e)
        {
            CloseNumericPadWithEnter(e);
        }

        private void pbPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            CloseNumericPad();
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            if (pbPassword.Password.ToString() == configuracion.GetPassword())
            {
                AutoIt.ProcessClose("infrared.exe");
                AutoIt.ProcessWaitClose("infrared.exe", 1);
                AutoIt.WinClose("VIVE", "");
            }
            else { MessageBoxResult messBox = MessageBox.Show("El password ingresado es incorrecto", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void btnApagar_Click(object sender, RoutedEventArgs e)
        {
           System.Windows.Forms.DialogResult messBox = System.Windows.Forms.MessageBox.Show("Está seguro que desea apagar el Equipo?", "Confirmacion", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Asterisk);
            if (messBox == System.Windows.Forms.DialogResult.Yes)
            {
                System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo("cmd", "/c shutdown -s -t 0");
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo = info;
                process.Start();
            }
        }
        #endregion

        #region Timers
        private void Timer_AutoIt_(object sender, EventArgs e)
        {
            if (AutoItTarget == "VIVE" || AutoItTarget == "NumericPad" )
            {
                //Manterner la ventana del juego de primera
                AutoIt.WinSetOnTop(AutoItTarget, "", 1);
                //Mostrar pantalla
                AutoIt.WinSetState(AutoItTarget, "", AutoIt.SW_SHOW);

                if(AutoItTarget == "VIVE") AutoIt.WinSetState(AutoItTarget, "", AutoIt.SW_MAXIMIZE);
            }
            else
            {
                if (System.Windows.Forms.Screen.AllScreens.Count() > 1)
                    //Posicionar ventana en la pantalla segundaria
                    AutoIt.WinMove(AutoItTarget, "", (int)System.Windows.SystemParameters.PrimaryScreenWidth, 0);   
                //Manterner la ventana del juego de primera
                AutoIt.WinSetOnTop(AutoItTarget, "", 1);
                //Mostrar pantalla
                AutoIt.WinSetState(AutoItTarget, "", AutoIt.SW_SHOW);
                AutoIt.WinSetState(AutoItTarget, "", AutoIt.SW_MAXIMIZE);

            }
            
            
        }

        private void Timer_(object sender, EventArgs e)
        {
            //Decontar tiempo del timer cuando se esta jugando
            txbTimer.Text = time.ToString(@"mm\:ss");
            if (time == TimeSpan.Zero)
            {
                if (tiempoCargaFinalizado)//Si el tiempo de carga ya habia finalizado
                {

                    //Incrementar reporte de reproduccion y Guardar venta
                    if (!IsCortesia)
                    {
                        reporte.IncreaseReproduccionesReport(gameSelected[0], tiempoCargado);
                        configuracion.SaveSale(tiempoCargado);

                        txbTotalVenta.Text = "El total de la venta es: $ " + configuracion.GetBaseRate() * tiempoCargado;
                    }
                    else
                    {
                        reporte.IncreaseCortesiasReport(gameSelected[0], tiempoCargado);
                        configuracion.RegisterCortesias();
                    }

                    //Cerrar en juego
                    AutoIt.WinKill(AutoItTarget);
                    AutoItTarget = "VIVE";

                    //Detener el timer
                    Timer.Stop();

                    //Quitar check al checkbox de cortesias
                    chkCortesia.IsChecked = false;
                    IsCortesia = false;

                    //Cambiar habilitacion de botones
                    btnAgregar.IsEnabled = false;
                    btnStop.IsEnabled = false;
                    btnTeamViewer.IsEnabled = true;
                    btnPlay.IsEnabled = true;
                }
                else    //Si no habia finalizado el tiempo de carga
                {
                    //Cargue el tiempo de juego
                    time = TimeSpan.FromMinutes(tiempoCargado);

                    //Finalice el tiempo de carga
                    tiempoCargaFinalizado = true;

                    //Habilitar Boton agregar
                    btnAgregar.IsEnabled = true;
                }
            }
            time = time.Add(TimeSpan.FromSeconds(-1));
        }
        #endregion

        #region Show Menus
        private void ShowReproduccionesMenu(object sender, RoutedEventArgs e)
        {
            //Colapsar todos los menus
            CollapseCajaMenu();
            CollapseSalirMenu();

            //Mostrar menu de reproducciones
            txbTimer.Visibility = Visibility.Visible;
            txbTiempo.Visibility = Visibility.Visible;
            txbTiempo2.Visibility = Visibility.Visible;
            txtTiempo.Visibility = Visibility.Visible;
            btnPlay.Visibility = Visibility.Visible;
            btnStop.Visibility = Visibility.Visible;
            txbJuegos.Visibility = Visibility.Visible;
            txtJuegos.Visibility = Visibility.Visible;
            txbJuegos2.Visibility = Visibility.Visible;
            chkCortesia.Visibility = Visibility.Visible;
            btnTeamViewer.Visibility = Visibility.Visible;
            btnAgregar.Visibility = Visibility.Visible;
            txbTotalVenta.Visibility = Visibility.Visible;
        }

        private void ShowCajaMenu(object sender, RoutedEventArgs e)
        {
            //Colapsar todos los menus
            CollapseReproduccionesMenu();
            CollapseSalirMenu();

            //Mostrar menu de caja
            lstMenuCaja.Visibility = Visibility.Visible;
            Rectangulo1.Visibility = Visibility.Visible;
            Rectangulo2.Visibility = Visibility.Visible;
            Rectangulo3.Visibility = Visibility.Visible;
            lblBaseEfectivoSistema.Visibility = Visibility.Visible;
            lblIDOperario.Visibility = Visibility.Visible;
            lblEfectivoBaseOperario.Visibility = Visibility.Visible;
            txbBaseEfectivoSistema.Visibility = Visibility.Visible;
            txtIDOperario.Visibility = Visibility.Visible;
            txtEfectivoBaseOperario.Visibility = Visibility.Visible;
            btnRegistrarApertura.Visibility = Visibility.Visible;

            //Reinicializar menu de caja y cajas de texto
            lstMenuCaja.SelectedIndex = 0;
            txtIDOperario.Text = "Ingrese su ID";
            txtEfectivoBaseOperario.Text = "Ingrese el efectivo base";

            //Mostrar Variables del sistema
            txbBaseEfectivoSistema.Text = "$ " + configuracion.GetCashBasis().ToString();
        }

        private void ShowSalirMenu(object sender, RoutedEventArgs e)
        {
            //Colapsar todos los menus
            CollapseReproduccionesMenu();
            CollapseCajaMenu();

            //Mostrar menu de salir
            lblPassword.Visibility = Visibility.Visible;
            pbPassword.Visibility = Visibility.Visible;
            btnSalir.Visibility = Visibility.Visible;
            btnApagar.Visibility = Visibility.Visible;

            //Reinicializar caja de texto de password
            pbPassword.Clear();
        }

        private void ShowAperturaMenu()
        {
            //Colapsar todos los menus
            CollapseCajaMenu();

            //Mostrar menu de Apertura
            lstMenuCaja.Visibility = Visibility.Visible;
            Rectangulo1.Visibility = Visibility.Visible;
            Rectangulo2.Visibility = Visibility.Visible;
            Rectangulo3.Visibility = Visibility.Visible;
            lblBaseEfectivoSistema.Visibility = Visibility.Visible;
            lblIDOperario.Visibility = Visibility.Visible;
            lblEfectivoBaseOperario.Visibility = Visibility.Visible;
            txbBaseEfectivoSistema.Visibility = Visibility.Visible;
            txtIDOperario.Visibility = Visibility.Visible;
            txtEfectivoBaseOperario.Visibility = Visibility.Visible;
            btnRegistrarApertura.Visibility = Visibility.Visible;

            //Reinicializar cajas de texto
            txtIDOperario.Text = "Ingrese su ID";
            txtEfectivoBaseOperario.Text = "Ingrese el efectivo base";

            //Mostrar Variables del sistema
            txbBaseEfectivoSistema.Text = "$ " + configuracion.GetCashBasis().ToString();
        }

        private void ShowCierreMenu()
        {
            //Colapsar todos los menus
            CollapseCajaMenu();

            //Mostrar menu de Cierre
            lstMenuCaja.Visibility = Visibility.Visible;
            Rectangulo1.Visibility = Visibility.Visible;
            Rectangulo2.Visibility = Visibility.Visible;
            Rectangulo3.Visibility = Visibility.Visible;
            lblEgresosJornadaSistema.Visibility = Visibility.Visible;
            lblEfectivoJornadaSistema.Visibility = Visibility.Visible;
            lblTotalMinutos.Visibility = Visibility.Visible;
            lblTotalReproducciones.Visibility = Visibility.Visible;
            lblEfectivoJornadaOperario.Visibility = Visibility.Visible;
            txbEgresosJornadaSistema.Visibility = Visibility.Visible;
            txbEfectivoJornadaSistema.Visibility = Visibility.Visible;
            txbTotalMinutos.Visibility = Visibility.Visible;
            txbTotalReproducciones.Visibility = Visibility.Visible;
            txtEfectivoJornadaOperario.Visibility = Visibility.Visible;
            btnRegistrarCierre.Visibility = Visibility.Visible;

            //Reinicializar cajas de texto
            txtEfectivoJornadaOperario.Text = "Ingrese el efectivo";

            //Mostrar valores de variables
            txbEgresosJornadaSistema.Text = "$ " + configuracion.GetDayEgress().ToString();
            txbEfectivoJornadaSistema.Text = "$ " + (configuracion.GetDayCash() + configuracion.GetCashBasis() - configuracion.GetDayEgress()).ToString();
            txbTotalMinutos.Text = configuracion.GetMinutesSold().ToString();
            txbTotalReproducciones.Text = configuracion.GetReproduccionesNumber().ToString();
        }

        private void ShowEgresoMenu()
        {
            //Colapsar todos los menus
            CollapseCajaMenu();

            //Mostrar menu de egresos
            lstMenuCaja.Visibility = Visibility.Visible;
            Rectangulo1.Visibility = Visibility.Visible;
            Rectangulo2.Visibility = Visibility.Visible;
            Rectangulo3.Visibility = Visibility.Visible;
            lblFechaEgresos.Visibility = Visibility.Visible;
            lblHoraEgresos.Visibility = Visibility.Visible;
            lblIDReceptorEgreso.Visibility = Visibility.Visible;
            lblValorEgreso.Visibility = Visibility.Visible;
            lblTipoEgreso.Visibility = Visibility.Visible;
            txbFechaEgresos.Visibility = Visibility.Visible;
            txbHoraEgresos.Visibility = Visibility.Visible;
            txtIDReceptorEgreso.Visibility = Visibility.Visible;
            txtValorEgreso.Visibility = Visibility.Visible;
            lwcboTipoEgreso.Visibility = Visibility.Visible;
            btnRegistarEgreso.Visibility = Visibility.Visible;

            //Reinicializar cajas de texto
            txtIDReceptorEgreso.Text = "Ingrese el ID";
            txtValorEgreso.Text = "Ingrese el valor";

            //Mostrar variables de egresos
            txbFechaEgresos.Text = System.DateTime.Now.ToString("dd'/'MM'/'yyyy", CultureInfo.CurrentUICulture.DateTimeFormat);
            txbHoraEgresos.Text = DateTime.Now.ToString("HH':'mm':'ss", CultureInfo.CurrentUICulture.DateTimeFormat);
        }

        private void ShowArqueoMenu()
        {
            //Colapsar todos los menus
            CollapseCajaMenu();

            //Mostrar menu de Arqueo
            lstMenuCaja.Visibility = Visibility.Visible;
            Rectangulo1.Visibility = Visibility.Visible;
            Rectangulo2.Visibility = Visibility.Visible;
            Rectangulo3.Visibility = Visibility.Visible;
            lblIngresosArqueoSistema.Visibility = Visibility.Visible;
            lblEgresosArqueoSistema.Visibility = Visibility.Visible;
            lblEfectivoArqueoSistema.Visibility = Visibility.Visible;
            lblReproduccionesFacturadasArqueo.Visibility = Visibility.Visible;
            lblReproduccionesErrorArqueo.Visibility = Visibility.Visible;
            lblReproduccionesCortesiaArqueo.Visibility = Visibility.Visible;
            txbIngresosArqueoSistema.Visibility = Visibility.Visible;
            txbEgresosArqueoSistema.Visibility = Visibility.Visible;
            txbEfectivoArqueoSistema.Visibility = Visibility.Visible;
            txbReproduccionesFacturadasArqueo.Visibility = Visibility.Visible;
            txbReproduccionesErrorArqueo.Visibility = Visibility.Visible;
            txbReproduccionesCortesiaArqueo.Visibility = Visibility.Visible;

            //Mostrar variables de arqueo
            txbIngresosArqueoSistema.Text = "$ " + configuracion.GetDayCash().ToString();
            txbEgresosArqueoSistema.Text = "$ " + configuracion.GetDayEgress().ToString();
            if (configuracion.IsCajaOpen())
                txbEfectivoArqueoSistema.Text = "$ " + (configuracion.GetDayCash() + configuracion.GetCashBasis() - configuracion.GetDayEgress()).ToString();
            else txbEfectivoArqueoSistema.Text = "$ " + 0.ToString();
            txbReproduccionesFacturadasArqueo.Text = configuracion.GetReproduccionesNumber().ToString();
            txbReproduccionesCortesiaArqueo.Text = configuracion.GetCortesiasNumber().ToString();
            txbReproduccionesErrorArqueo.Text = configuracion.GetErroresNumber().ToString();
            
        }


        #endregion

        #region Collapse
        private void CollapseReproduccionesMenu()
        {
            txbTimer.Visibility = Visibility.Collapsed;
            txbTiempo.Visibility = Visibility.Collapsed;
            txbTiempo2.Visibility = Visibility.Collapsed;
            txtTiempo.Visibility = Visibility.Collapsed;
            btnPlay.Visibility = Visibility.Collapsed;
            btnStop.Visibility = Visibility.Collapsed;
            txbJuegos.Visibility = Visibility.Collapsed;
            txtJuegos.Visibility = Visibility.Collapsed;
            txbJuegos2.Visibility = Visibility.Collapsed;
            chkCortesia.Visibility = Visibility.Collapsed;
            btnTeamViewer.Visibility = Visibility.Collapsed;
            btnAgregar.Visibility = Visibility.Collapsed;
            txbTotalVenta.Visibility = Visibility.Collapsed;
        }

        private void CollapseCajaMenu()
        {
            //Colapse Objetos generales del menu de caja
            lstMenuCaja.Visibility = Visibility.Collapsed;
            Rectangulo1.Visibility = Visibility.Collapsed;
            Rectangulo2.Visibility = Visibility.Collapsed;
            Rectangulo3.Visibility = Visibility.Collapsed;

            //Colapse Objetos del menu de Apertura
            lblBaseEfectivoSistema.Visibility = Visibility.Collapsed;
            lblIDOperario.Visibility = Visibility.Collapsed;
            lblEfectivoBaseOperario.Visibility = Visibility.Collapsed;
            txbBaseEfectivoSistema.Visibility = Visibility.Collapsed;
            txtIDOperario.Visibility = Visibility.Collapsed;
            txtEfectivoBaseOperario.Visibility = Visibility.Collapsed;
            btnRegistrarApertura.Visibility = Visibility.Collapsed;

            //Colapse Objetos del menu de Cierre
            lblEgresosJornadaSistema.Visibility = Visibility.Collapsed;
            lblEfectivoJornadaSistema.Visibility = Visibility.Collapsed;
            lblTotalMinutos.Visibility = Visibility.Collapsed;
            lblTotalReproducciones.Visibility = Visibility.Collapsed;
            lblEfectivoJornadaOperario.Visibility = Visibility.Collapsed;
            txbEgresosJornadaSistema.Visibility = Visibility.Collapsed;
            txbEfectivoJornadaSistema.Visibility = Visibility.Collapsed;
            txbTotalMinutos.Visibility = Visibility.Collapsed;
            txbTotalReproducciones.Visibility = Visibility.Collapsed;
            txtEfectivoJornadaOperario.Visibility = Visibility.Collapsed;
            btnRegistrarCierre.Visibility = Visibility.Collapsed;

            //Colapse Objetos del menu de Egresos
            lblFechaEgresos.Visibility = Visibility.Collapsed;
            lblHoraEgresos.Visibility = Visibility.Collapsed;
            lblIDReceptorEgreso.Visibility = Visibility.Collapsed;
            lblValorEgreso.Visibility = Visibility.Collapsed;
            lblTipoEgreso.Visibility = Visibility.Collapsed;
            txbFechaEgresos.Visibility = Visibility.Collapsed;
            txbHoraEgresos.Visibility = Visibility.Collapsed;
            txtIDReceptorEgreso.Visibility = Visibility.Collapsed;
            txtValorEgreso.Visibility = Visibility.Collapsed;
            lwcboTipoEgreso.Visibility = Visibility.Collapsed;
            btnRegistarEgreso.Visibility = Visibility.Collapsed;

            //Colapse Objetos del menu de Arqueos
            lblIngresosArqueoSistema.Visibility = Visibility.Collapsed;
            lblEgresosArqueoSistema.Visibility = Visibility.Collapsed;
            lblEfectivoArqueoSistema.Visibility = Visibility.Collapsed;
            lblReproduccionesFacturadasArqueo.Visibility = Visibility.Collapsed;
            lblReproduccionesErrorArqueo.Visibility = Visibility.Collapsed;
            lblReproduccionesCortesiaArqueo.Visibility = Visibility.Collapsed;
            txbIngresosArqueoSistema.Visibility = Visibility.Collapsed;
            txbEgresosArqueoSistema.Visibility = Visibility.Collapsed;
            txbEfectivoArqueoSistema.Visibility = Visibility.Collapsed;
            txbReproduccionesFacturadasArqueo.Visibility = Visibility.Collapsed;
            txbReproduccionesErrorArqueo.Visibility = Visibility.Collapsed;
            txbReproduccionesCortesiaArqueo.Visibility = Visibility.Collapsed;
        }

        private void CollapseSalirMenu()
        {
            lblPassword.Visibility = Visibility.Collapsed;
            pbPassword.Visibility = Visibility.Collapsed;
            btnSalir.Visibility = Visibility.Collapsed;
            btnApagar.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region Support funtions
        private void CloseNumericPad()
        {
            if (!FocusManager.GetFocusedElement(this).GetType().Equals(typeof(TextBox)))
            {
                if (AutoIt.WinExists("NumericPad", "") == 1)
                {
                    AutoIt.WinClose("NumericPad", "");
                }
                AutoItTarget = "VIVE";
                AutoIt.WinSetOnTop(AutoItTarget, "", 1);
            }
        }

        private void CloseNumericPadWithEnter(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (AutoIt.WinExists("NumericPad", "") == 1)
                {
                    AutoIt.WinClose("NumericPad", "");
                }
                AutoItTarget = "VIVE";
                AutoIt.WinSetOnTop(AutoItTarget, "", 1);

                TraversalRequest tRequest = new TraversalRequest(FocusNavigationDirection.Next);
                UIElement keyboardFocus = Keyboard.FocusedElement as UIElement;

                if (keyboardFocus != null)
                {
                    keyboardFocus.MoveFocus(tRequest);
                }

                e.Handled = true;
            }
        }

        private void OpenNumericPad()
        {  
            AutoItTarget = "NumericPad";
            if (AutoIt.WinExists("NumericPad", "") == 0)
            {
                int PID = AutoIt.Run("NumericPad/NumericPad.exe", "", 1);
                if(PID != 0)AutoIt.WinWait(AutoItTarget);
                else { MessageBoxResult messBox = MessageBox.Show("El NumericPad no se pudo abrir", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Error); }
            }
            AutoIt.WinSetOnTop(AutoItTarget, "", 1);
        }

        private bool GameExists(string gameId)
        {
            foreach(string[] game in games)
            {
                if (game[0] == gameId) return true;
            }
            return false;
        }

        private string[] GetGameSelected(string gameId)
        {
            foreach (string[] game in games)
            {
                if (game[0] == gameId) return game;
            }
            return null;
        }


        #endregion
    }
}


