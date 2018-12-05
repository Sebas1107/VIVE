using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;
using System.Security.Permissions;
using System.Windows;

namespace VIVE
{
    [UIPermission(SecurityAction.Demand, Unrestricted = true)]
    class Configuracion
    {
        static int reproduccionesSerialNumber, jornadasSerialNumber, egresosSerialNumber, errorsSerialNumber, cortesiasSerialNumber;
        static int baseRate, cashBasis, minutesSold, reproduccionesNumber, cortesiasNumber, erroresNumber, dayCash, dayEgress;
        static string cajaState, idLastOperario, password;
        static List<string[]> operarios = new List<string[]>(), juegos = new List<string[]>();
        string configFile = "Configuracion.txt", usuariosFile = "Usuarios.txt", juegosFile = "Juegos.txt";

        #region Public funtions
        //Cargar Configuracion
        public void LoadConfig()
        {
            //Cargar variables del sistema
            try
            {
                var linesC = File.ReadAllLines(configFile, Encoding.Default);
                foreach (string line in linesC)
                {
                    var lineSplit = line.Split(':');
                    if (lineSplit[0] == "ConsecutivoReproducciones")
                    {
                        reproduccionesSerialNumber = int.Parse(lineSplit[1]);
                    }
                    if (lineSplit[0] == "ConsecutivoJornadas")
                    {
                        jornadasSerialNumber = int.Parse(lineSplit[1]);
                    }
                    if (lineSplit[0] == "ConsecutivoEgresos")
                    {
                        egresosSerialNumber = int.Parse(lineSplit[1]);
                    }
                    if (lineSplit[0] == "ConsecutivoErrores")
                    {
                        errorsSerialNumber = int.Parse(lineSplit[1]);
                    }
                    if (lineSplit[0] == "ConsecutivoCortesias")
                    {
                        cortesiasSerialNumber = int.Parse(lineSplit[1]);
                    }
                    if (lineSplit[0] == "Tarifa")
                    {
                        baseRate = int.Parse(lineSplit[1]);
                    }
                    if (lineSplit[0] == "EfectivoBase")
                    {
                        cashBasis = int.Parse(lineSplit[1]);
                    }
                    if (lineSplit[0] == "MinutosJornada")
                    {
                        minutesSold = int.Parse(lineSplit[1]);
                    }
                    if (lineSplit[0] == "NumeroReproducciones")
                    {
                        reproduccionesNumber = int.Parse(lineSplit[1]);
                    }
                    if (lineSplit[0] == "NumeroCortesias")
                    {
                        cortesiasNumber = int.Parse(lineSplit[1]);
                    }
                    if (lineSplit[0] == "NumeroErrores")
                    {
                        erroresNumber = int.Parse(lineSplit[1]);
                    }
                    if (lineSplit[0] == "UltimoEstadoCaja")
                    {
                        cajaState = lineSplit[1];
                    }
                    if (lineSplit[0] == "EgresosJornada")
                    {
                        dayEgress = int.Parse(lineSplit[1]);
                    }
                    if (lineSplit[0] == "IngresosJornada")
                    {
                        dayCash = int.Parse(lineSplit[1]);
                    }
                    if (lineSplit[0] == "IdUltimoOperario")
                    {
                        idLastOperario = lineSplit[1];
                    }
                    if (lineSplit[0] == "Password")
                    {
                        password = lineSplit[1];
                    }
                }
            }
            catch { MessageBoxResult messBox = MessageBox.Show("El archivo de Configuracion no se puede leer", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Error); }

            try
            {
                //Cargar Usuarios
                var linesO = File.ReadAllLines(usuariosFile, Encoding.Default);
                for (int i = 1; i < linesO.Count(); i++)
                {
                    operarios.Add(linesO[i].Split(','));
                }
            }
            catch { MessageBoxResult messBox = MessageBox.Show("El archivo de Usuarios no se puede leer", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Error); }

            try
            {
                //Cargar Juegos
                var linesJ = File.ReadAllLines(juegosFile, Encoding.Default);
                for (int i = 1; i < linesJ.Count(); i++)
                {

                    juegos.Add(linesJ[i].Split(','));
                }
            }
            catch(Exception exception)
            {
                App.SendReport(exception);
                MessageBoxResult messBox = MessageBox.Show("El archivo de Juegos no se puede leer", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //Generador de numeros seriales
        public string GenerateSerialNumber(string tabla)
        {
            if (tabla == "Reproducciones")
            {
                reproduccionesSerialNumber += 1;
                var lines = File.ReadAllLines(configFile);
                for (int i = 0; i < lines.Count(); i++)
                {
                    if (lines[i].Contains("ConsecutivoReproducciones")) lines[i] = "ConsecutivoReproducciones:" + reproduccionesSerialNumber;
                }
                File.WriteAllLines(configFile, lines);
                
                return reproduccionesSerialNumber.ToString("000000");
            }
            if (tabla == "Jornadas")
            {
                jornadasSerialNumber += 1;
                var lines = File.ReadAllLines(configFile);
                for (int i = 0; i < lines.Count(); i++)
                {
                    if (lines[i].Contains("ConsecutivoJornadas")) lines[i] = "ConsecutivoJornadas:" + jornadasSerialNumber;
                }
                File.WriteAllLines(configFile, lines);

                return jornadasSerialNumber.ToString("000000");
            }
            if (tabla == "Egresos")
            {
                egresosSerialNumber += 1;
                var lines = File.ReadAllLines(configFile);
                for (int i = 0; i < lines.Count(); i++)
                {
                    if (lines[i].Contains("ConsecutivoEgresos")) lines[i] = "ConsecutivoEgresos:" + egresosSerialNumber;
                }
                File.WriteAllLines(configFile, lines);

                return egresosSerialNumber.ToString("000000");
            }
            if (tabla == "Errores")
            {
                errorsSerialNumber += 1;
                var lines = File.ReadAllLines(configFile);
                for (int i = 0; i < lines.Count(); i++)
                {
                    if (lines[i].Contains("ConsecutivoErrores")) lines[i] = "ConsecutivoErrores:" + errorsSerialNumber;
                }
                File.WriteAllLines(configFile, lines);

                return errorsSerialNumber.ToString("000000");
            }
            if (tabla == "Cortesias")
            {
                cortesiasSerialNumber += 1;
                var lines = File.ReadAllLines(configFile);
                for (int i = 0; i < lines.Count(); i++)
                {
                    if (lines[i].Contains("ConsecutivoCortesias")) lines[i] = "ConsecutivoCortesias:" + cortesiasSerialNumber;
                }
                File.WriteAllLines(configFile, lines);

                return cortesiasSerialNumber.ToString("000000");
            }
            return "";

        }

        //Guardar venta
        public void SaveSale(int time)
        {
            minutesSold += time;
            reproduccionesNumber += 1;
            dayCash += time * baseRate;
            var lines = File.ReadAllLines(configFile);
            for (int i = 0; i < lines.Count(); i++)
            {
                if (lines[i].Contains("MinutosJornada")) lines[i] = "MinutosJornada:" + minutesSold;
                if (lines[i].Contains("NumeroReproducciones")) lines[i] = "NumeroReproducciones:" + reproduccionesNumber;
                if (lines[i].Contains("IngresosJornada")) lines[i] = "IngresosJornada:" + dayCash;
            }
            File.WriteAllLines(configFile, lines);
        }

        //Comprobar estado de caja
        public bool IsCajaOpen()
        {
            if (cajaState == "Abierta") { return true; }
            return false;
        }

        //Abrir caja
        public bool OpenCaja(string id, string baseEfectivo)
        {
            if (!IsCajaOpen())
            {
                if (OperarioExists(id))
                {
                    int b;
                    if(int.TryParse(baseEfectivo, out b))
                    {
                        //Guarde el estado de caja
                        SaveCajaState("Abierta");

                        //Guarde el id del operario
                        idLastOperario = id;
                        var lines = File.ReadAllLines(configFile);
                        for (int i = 0; i < lines.Count(); i++)
                        {
                            if (lines[i].Contains("IdUltimoOperario")) lines[i] = "IdUltimoOperario:" + id;
                        }
                        File.WriteAllLines(configFile, lines);

                        return true;
                    }
                    else { MessageBoxResult messBox = MessageBox.Show("El efectivo base ingresado no es un numero entero", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Error); }
                }
                else { MessageBoxResult messBox = MessageBox.Show("El id del operario ingresado no existe", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Error); }
            }
            else  {  MessageBoxResult messBox = MessageBox.Show("La caja ya esta abierta", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Exclamation);  }

            return false;
        }

        //Cerrar caja
        public bool CloseCaja(string efectivo)
        {
            if (IsCajaOpen())
            {
                int b;
                if (int.TryParse(efectivo, out b))
                {
                    //Guardar y resetear todas las variables de cierre de caja
                    cashBasis = dayCash + cashBasis - dayEgress;
                    minutesSold = 0;
                    reproduccionesNumber = 0;
                    dayEgress = 0;
                    dayCash = 0;
                    cortesiasNumber = 0;
                    erroresNumber = 0;
                    var lines = File.ReadAllLines(configFile);
                    for (int i = 0; i < lines.Count(); i++)
                    {
                        if (lines[i].Contains("EgresosJornada")) lines[i] = "EgresosJornada:" + dayEgress;
                        if (lines[i].Contains("EfectivoBase")) lines[i] = "EfectivoBase:" + cashBasis;
                        if (lines[i].Contains("MinutosJornada")) lines[i] = "MinutosJornada:" + minutesSold;
                        if (lines[i].Contains("NumeroReproducciones")) lines[i] = "NumeroReproducciones:" + reproduccionesNumber;
                        if (lines[i].Contains("IngresosJornada")) lines[i] = "IngresosJornada:" + dayCash;
                        if (lines[i].Contains("NumeroCortesias")) lines[i] = "NumeroCortesias:" + cortesiasNumber;
                        if (lines[i].Contains("NumeroErrores")) lines[i] = "NumeroErrores:" + erroresNumber;
                    }
                    File.WriteAllLines(configFile, lines);

                    //Guarde el estado de caja
                    SaveCajaState("Cerrada");

                    return true;
                }
                else { MessageBoxResult messBox = MessageBox.Show("El efectivo ingresado no es un numero entero", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Error); }
            }
            else { MessageBoxResult messBox = MessageBox.Show("La caja ya esta cerrada", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Exclamation); }

            return false;
        }

        //Registar egreso
        public bool RegisterEgress(string idReceptor, string egressValue)
        {
            if (IsCajaOpen())
            {
                if (AdministradorExists(idReceptor))
                {
                    int egress;
                    if (int.TryParse(egressValue, out egress))
                    {
                        //Guarde el egreso
                        dayEgress += egress;
                        var lines = File.ReadAllLines(configFile);
                        for (int i = 0; i < lines.Count(); i++)
                        {
                            if (lines[i].Contains("EgresosJornada")) lines[i] = "EgresosJornada:" + dayEgress;
                        }
                        File.WriteAllLines(configFile, lines);

                        return true;
                    }
                    else { MessageBoxResult messBox = MessageBox.Show("El egreso ingresado no es un numero entero", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Error); }
                }
                else { MessageBoxResult messBox = MessageBox.Show("El id del administrador ingresado no existe", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Error); }
            }
            else { MessageBoxResult messBox = MessageBox.Show("La caja no ha sido abierta, por favor abra la caja", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Exclamation); }

            return false;
        }

        //Registre cortesia
        public void RegisterCortesias()
        {
            cortesiasNumber += 1;
            var lines = File.ReadAllLines(configFile);
            for (int i = 0; i < lines.Count(); i++)
            {
                if (lines[i].Contains("NumeroCortesias")) lines[i] = "NumeroCortesias:" + cortesiasNumber;
            }
            File.WriteAllLines(configFile, lines);
        }

        //Registrar Error
        public void RegisterError()
        {
            erroresNumber += 1;
            var lines = File.ReadAllLines(configFile);
            for (int i = 0; i < lines.Count(); i++)
            {
                if (lines[i].Contains("NumeroErrores")) lines[i] = "NumeroErrores:" + erroresNumber;
            }
            File.WriteAllLines(configFile, lines);
        }
        #endregion

        #region Get functions
        //Devuelve el valor de la tarifa
        public int GetBaseRate()
        {
            return baseRate;
        }

        //Devuelve el valor de la base de efectivo
        public int GetCashBasis()
        {
            return cashBasis;
        }

        //Devuelve el valor de los egresos del dia
        public int GetDayEgress()
        {
            return dayEgress;
        }

        //Devuelve el valor de los ingresos del dia
        public int GetDayCash()
        {
            return dayCash;
        }

        //Devuelve el numero de minutos del dia
        public int GetMinutesSold()
        {
            return minutesSold;
        }

        //Devuelve el numero de reproducciones del dia
        public int GetReproduccionesNumber()
        {
            return reproduccionesNumber;
        }

        //Devuelve el Id del operario
        public string GetIdLastOperario()
        {
            return idLastOperario;
        }

        //Devuelve el numero de cortesias del dia
        public int GetCortesiasNumber()
        {
            return cortesiasNumber;
        }

        //Devuelve el numero de errores del dia
        public int GetErroresNumber()
        {
            return erroresNumber;
        }

        //Devuelve el password
        public string GetPassword()
        {
            return password;
        }

        //Devuelve los juegos
        public List<string[]> GetGames()
        {
            return juegos;
        }
        #endregion

        //Comprobar si el operario existe
        private bool OperarioExists(string id)
        {
            foreach (string[] operario in operarios)
            {
                if (operario[2] == id) { return true; }
            }
            return false;
        }

        //Comprobar si el administrador existe
        private bool AdministradorExists(string id)
        {
            foreach (string[] operario in operarios)
            {
                if (operario[2] == id) { return true; }
            }
            return false;
        }

        //Guardar elestado de caja
        private void SaveCajaState(string state)
        {
            cajaState = state;
            var lines = File.ReadAllLines(configFile);
            for (int i = 0; i < lines.Count(); i++)
            {
                if (lines[i].Contains("UltimoEstadoCaja")) lines[i] = "UltimoEstadoCaja:" + state;
            }
            File.WriteAllLines(configFile, lines);
        }

    }
}
