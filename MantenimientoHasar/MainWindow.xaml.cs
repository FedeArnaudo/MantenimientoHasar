using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using MessageBox = System.Windows.Forms.MessageBox;

namespace MantenimientoHasar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private NotifyIcon notifyIcon;
        private readonly DispatcherTimer dispatcherTimer;
        public MainWindow()
        {
            InitializeComponent();
            Icon = new BitmapImage(new Uri("pack://application:,,,/MantenimientoHasar;component/MantenimientoHasar.ico"));
            SetupNotifyIcon();
            dispatcherTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromHours(2)
            };
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Start();
        }
        private void btnComenzarMantenimiento_Click(object sender, RoutedEventArgs e)
        {
            if (!(TextBoxRutaProyecto.Text == null || TextBoxRutaProyecto.Text == ""))
            {
                DatosTxt.InstanciaDatosTxt().VerificarCarpeta();
                DatosTxt.InstanciaDatosTxt().Escribir(DatosTxt.fileProy, TextBoxRutaProyecto.Text);
                DatosTxt.InstanciaDatosTxt().Escribir(DatosTxt.fileDelete, TextBoxArchivo.Text);

                ExecuteMethod(TextBoxRutaProyecto.Text);
            }
            else
            {
                _ = MessageBox.Show($"Debe ingresar la ruta del proyecto");
            }
        }
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            List<string> archivos = DatosTxt.InstanciaDatosTxt().Listar(DatosTxt.fileProy);
            if (archivos.Count == 1)
            {
                // Aquí se ejecuta el método cada vez que el DispatcherTimer hace tick
                ExecuteMethod(archivos[0]);
            }
        }
        private void ExecuteMethod(string ruta)
        {
            DatosTxt.InstanciaDatosTxt().EliminarArchivos(ruta);

        }
        private void btnBuscarRuta_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog
            {
                RootFolder = Environment.SpecialFolder.MyComputer, // Carpeta raíz (opcional)
                Description = "Selecciona una carpeta" // Descripción del diálogo (opcional)
            };

            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string folderPath = folderBrowserDialog.SelectedPath;
                TextBoxRutaProyecto.Text = folderPath;
            }
        }
        private void SetupNotifyIcon()
        {
            notifyIcon = new NotifyIcon
            {
                Icon = new Icon("MantenimientoHasar2.ico"),
                Visible = true,
                Text = "Mantenimiento Hasar"
            };

            notifyIcon.DoubleClick += NotifyIcon_DoubleClick;

            ContextMenu contextMenu = new ContextMenu();
            _ = contextMenu.MenuItems.Add("Restaurar", (s, e) => RestoreFromTray());
            _ = contextMenu.MenuItems.Add("Salir", (s, e) => ExitApplication());

            notifyIcon.ContextMenu = contextMenu;
        }
        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            RestoreFromTray();
        }
        private void RestoreFromTray()
        {
            Show();
            WindowState = WindowState.Normal;
            notifyIcon.Visible = false;
        }
        private void ExitApplication()
        {
            notifyIcon.Visible = false;
            System.Windows.Application.Current.Shutdown();
        }

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);

            if (WindowState == WindowState.Minimized)
            {
                Hide();
                notifyIcon.Visible = true;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            notifyIcon.Dispose();
            base.OnClosed(e);
        }
    }
}
