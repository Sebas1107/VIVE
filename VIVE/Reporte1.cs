
using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;
using System.Security.Permissions;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Windows.Media.Imaging;

namespace VIVE
{
    [UIPermission(SecurityAction.Demand, Unrestricted = true)]
    class Reporte1
    {
        Configuracion configuracion = new Configuracion();
        static string reportMonthDate;
        static string reproduccionesFile, aperturasFile, cierresFile, egresosFile, cortesiasFile, erroresFile;

        //Genera las carpetas necesarias para el reporte basico
        public void GenerateReport()
        {
            reportMonthDate = System.DateTime.Now.ToString("MM'-'yyyy", CultureInfo.CurrentUICulture.DateTimeFormat);

            string directory;
            reproduccionesFile = @"Reportes/Reproducciones/Reproducciones-" + reportMonthDate + ".csv";

            if (!File.Exists(reproduccionesFile))
            {
                directory = @"Reportes";
                if (!Directory.Exists(directory)) { Directory.CreateDirectory(directory); }

                directory = @"Reportes/Reproducciones";
                if (!Directory.Exists(directory)) { Directory.CreateDirectory(directory); }

                FileStream fileStream = new FileStream(reproduccionesFile, FileMode.CreateNew);
                fileStream.Close();

                StreamWriter objWriter = new StreamWriter(reproduccionesFile);
                objWriter.WriteLine("Id Ingreso,Fecha,Hora,Id Juego,Minutos,Tarifa");
                objWriter.Close();
            }

            cierresFile = @"Reportes/Caja/Cierres/Cierres-" + reportMonthDate + ".csv";

            if (!File.Exists(cierresFile))
            {
                directory = @"Reportes/Caja";
                if (!Directory.Exists(directory)) { Directory.CreateDirectory(directory); }

                directory = @"Reportes/Caja/Cierres";
                if (!Directory.Exists(directory)) { Directory.CreateDirectory(directory); }

                FileStream fileStream = new FileStream(cierresFile, FileMode.CreateNew);
                fileStream.Close();

                StreamWriter objWriter = new StreamWriter(cierresFile);
                objWriter.WriteLine("Id Jornada,Fecha,Hora,Efectivo Jornada Sistema,Egresos Jornada Sistema,Id Usuario,Efectivo Jornada Operario");
                objWriter.Close();
            }

            aperturasFile = @"Reportes/Caja/Aperturas/Aperturas-" + reportMonthDate + ".csv";

            if (!File.Exists(aperturasFile))
            {
                directory = @"Reportes/Caja/Aperturas";
                if (!Directory.Exists(directory)) { Directory.CreateDirectory(directory); }

                FileStream fileStream = new FileStream(aperturasFile, FileMode.CreateNew);
                fileStream.Close();

                StreamWriter objWriter = new StreamWriter(aperturasFile);
                objWriter.WriteLine("Id Jornada,Fecha,Hora,Base Efectivo Sistema,Id Usuario,Base Efectivo Operario");
                objWriter.Close();
            }

            egresosFile = @"Reportes/Caja/Egresos/Egresos-" + reportMonthDate + ".csv";

            if (!File.Exists(egresosFile))
            {
                directory = @"Reportes/Caja/Egresos";
                if (!Directory.Exists(directory)) { Directory.CreateDirectory(directory); }

                FileStream fileStream = new FileStream(egresosFile, FileMode.CreateNew);
                fileStream.Close();

                StreamWriter objWriter = new StreamWriter(egresosFile);
                objWriter.WriteLine("Id Egreso,Fecha,Hora,Id Emisor,Id Receptor,Valor Egreso,Tipo Egreso");
                objWriter.Close();
            }

            cortesiasFile = @"Reportes/Cortesias/Cortesias-" + reportMonthDate + ".csv";

            if (!File.Exists(cortesiasFile))
            {
                directory = @"Reportes/Cortesias";
                if (!Directory.Exists(directory)) { Directory.CreateDirectory(directory); }

                FileStream fileStream = new FileStream(cortesiasFile, FileMode.CreateNew);
                fileStream.Close();

                StreamWriter objWriter = new StreamWriter(cortesiasFile);
                objWriter.WriteLine("Id Cortesias,Fecha,Hora,Id Juego,Minutos");
                objWriter.Close();
            }

            erroresFile = @"Reportes/Errores/Errores-" + reportMonthDate + ".csv";

            if (!File.Exists(erroresFile))
            {
                directory = @"Reportes/Errores";
                if (!Directory.Exists(directory)) { Directory.CreateDirectory(directory); }

                FileStream fileStream = new FileStream(erroresFile, FileMode.CreateNew);
                fileStream.Close();

                StreamWriter objWriter = new StreamWriter(erroresFile);
                objWriter.WriteLine("Id Error,Fecha,Hora,Id Juego,Id Usuario");
                objWriter.Close();
            }
        } 

        //Incrementa reporte de reproducciones
        public void IncreaseReproduccionesReport(string gameID, int gameTime)
        {
            string date = System.DateTime.Now.ToString("dd'/'MM'/'yyyy", CultureInfo.CurrentUICulture.DateTimeFormat);
            string time = DateTime.Now.ToString("HH':'mm':'ss", CultureInfo.CurrentUICulture.DateTimeFormat);

            string newLine = configuracion.GenerateSerialNumber("Reproducciones") + "," + date + "," + time + "," + gameID + "," + gameTime.ToString() + "," + configuracion.GetBaseRate().ToString();

            File.AppendAllText(reproduccionesFile, newLine + Environment.NewLine);

            SendReport("Reproducciones");
        }

        //Incremente reporte de apertura
        public void IncreaseAperturaReport(string id, string cashBasis)
        {
            string date = System.DateTime.Now.ToString("dd'/'MM'/'yyyy", CultureInfo.CurrentUICulture.DateTimeFormat);
            string time = DateTime.Now.ToString("HH':'mm':'ss", CultureInfo.CurrentUICulture.DateTimeFormat);

            string newLine = configuracion.GenerateSerialNumber("Jornadas") + "," + date + "," + time + "," + configuracion.GetCashBasis().ToString() + "," + id + "," + cashBasis;

            File.AppendAllText(aperturasFile, newLine + Environment.NewLine);

            SendReport("Aperturas");
        }

        //Incremente reporte de cierre
        public void IncreaseCierreReport(string dayCash)
        {
            string date = System.DateTime.Now.ToString("dd'/'MM'/'yyyy", CultureInfo.CurrentUICulture.DateTimeFormat);
            string time = DateTime.Now.ToString("HH':'mm':'ss", CultureInfo.CurrentUICulture.DateTimeFormat);

            string newLine = configuracion.GenerateSerialNumber("Jornadas") + "," + date + "," + time + "," + configuracion.GetDayCash().ToString() + "," + configuracion.GetDayEgress().ToString() +"," + configuracion.GetIdLastOperario() + "," + dayCash;

            File.AppendAllText(cierresFile, newLine + Environment.NewLine);

            SendReport("Cierres");
        }

        //Incremente reporte de egresos
        public void IncreaseEgresoReport(string idRepector, string egressValue, string egressType)
        {
            string date = System.DateTime.Now.ToString("dd'/'MM'/'yyyy", CultureInfo.CurrentUICulture.DateTimeFormat);
            string time = DateTime.Now.ToString("HH':'mm':'ss", CultureInfo.CurrentUICulture.DateTimeFormat);

            string newLine = configuracion.GenerateSerialNumber("Egresos") + "," + date + "," + time + "," + configuracion.GetIdLastOperario() + "," + idRepector + "," + egressValue + "," + egressType;

            File.AppendAllText(egresosFile, newLine + Environment.NewLine);

            SendReport("Egresos");
        }

        //Incrementa reporte de cortesias
        public void IncreaseCortesiasReport(string gameID, int gameTime)
        {
            string date = System.DateTime.Now.ToString("dd'/'MM'/'yyyy", CultureInfo.CurrentUICulture.DateTimeFormat);
            string time = DateTime.Now.ToString("HH':'mm':'ss", CultureInfo.CurrentUICulture.DateTimeFormat);

            string newLine = configuracion.GenerateSerialNumber("Cortesias") + "," + date + "," + time + "," + gameID + "," + gameTime.ToString();

            File.AppendAllText(cortesiasFile, newLine + Environment.NewLine);

            SendReport("Cortesias");
        }

        //Incrementa reporte de reproducciones
        public void IncreaseErrorReport(string gameID)
        {
            string date = System.DateTime.Now.ToString("dd'/'MM'/'yyyy", CultureInfo.CurrentUICulture.DateTimeFormat);
            string time = DateTime.Now.ToString("HH':'mm':'ss", CultureInfo.CurrentUICulture.DateTimeFormat);

            string newLine = configuracion.GenerateSerialNumber("Errores") + "," + date + "," + time + "," + gameID + "," + configuracion.GetIdLastOperario();

            File.AppendAllText(erroresFile, newLine + Environment.NewLine);

            SendReport("Errores");
        }

        //Enviar reporte al Drive
        public void SendReport(string tabla)
        {
            if (tabla == "Reproducciones")
            {
                try
                {
                    File.Copy(reproduccionesFile, "C:/Users/" + Environment.UserName + "/Google Drive/reproducciones/Reproducciones-" + reportMonthDate + ".csv", true);
                }
                catch { }
            }
            if (tabla == "Aperturas")
            {
                try
                {
                    File.Copy(aperturasFile, "C:/Users/" + Environment.UserName + "/Google Drive/caja/aperturas/Aperturas-" + reportMonthDate + ".csv", true);
                }
                catch { }
            }
            if (tabla == "Cierres")
            {
                try
                {
                    File.Copy(cierresFile, "C:/Users/" + Environment.UserName + "/Google Drive/caja/cierres/Cierres-" + reportMonthDate + ".csv", true);
                }
                catch { }
            }
            if (tabla == "Egresos")
            {
                try
                {
                    File.Copy(egresosFile, "C:/Users/" + Environment.UserName + "/Google Drive/caja/egresos/Egresos-" + reportMonthDate + ".csv", true);
                }
                catch { }
            }
            if (tabla == "Cortesias")
            {
                try
                {
                    File.Copy(cortesiasFile, "C:/Users/" + Environment.UserName + "/Google Drive/reproducciones/cortesias/Cortesias-" + reportMonthDate + ".csv", true);
                }
                catch { }
            }
            if (tabla == "Errores")
            {
                try
                {
                    File.Copy(erroresFile, "C:/Users/" + Environment.UserName + "/Google Drive/reproducciones/errores/Errores-" + reportMonthDate + ".csv", true);
                }
                catch { }
            }
        }
    }
}
