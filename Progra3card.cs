using System;
using MySql.Data.MySqlClient; 

namespace Progra3Card.Administrativo
{
    class Program
    {
        private static string connectionString = "Server=localhost;Database=mi_banco_db;Uid=root;Pwd=root;";

        static void Main(string[] args)
        {
            bool salir = false;
            while (!salir)
            {
                Console.Clear();
                Console.WriteLine("========================================");
                Console.WriteLine("    SISTEMA ADMINISTRATIVO PROGRA3CARD   ");
                Console.WriteLine("========================================");
                Console.WriteLine("1. Emitir Nueva Tarjeta (Alta de Cliente)");
                Console.WriteLine("2. Listar Tarjetas");
                Console.WriteLine("3. Ver Detalle de una Tarjeta / Cliente");
                Console.WriteLine("4. Eliminar Tarjeta (Baja de Sistema)");
                Console.WriteLine("5. Emitir Nueva Liquidación Mensual");
                Console.WriteLine("6. Salir");
                Console.WriteLine("========================================");
                Console.Write("Seleccione una opción: ");

                switch (Console.ReadLine())
                {
                    case "1": MenuEmitirTarjeta(); break;
                    case "2": MenuListarTarjetas(); break;
                    case "3": MenuVerDetalleTarjeta(); break;
                    case "4": MenuEliminarTarjeta(); break;
                    case "5": MenuEmitirLiquidacion(); break;
                    case "6": salir = true; break;
                    default:
                        Console.WriteLine("Opción no válida. Presione una tecla para continuar...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void MenuEmitirTarjeta()
        {
            Console.Clear();
            Console.WriteLine("--- EMITIR NUEVA TARJETA (ALTA DE CLIENTE) ---\n");

            Console.Write("Número de Documento (DNI/Pasaporte): ");
            string documento = Console.ReadLine();

            Console.Write("Tipo de documento (DNI / PASAPORTE): ");
            string tipoDoc = Console.ReadLine().ToUpper();
            while (tipoDoc != "DNI" && tipoDoc != "PASAPORTE")
            {
                Console.Write("Tipo inválido. Ingresá DNI o PASAPORTE: ");
                tipoDoc = Console.ReadLine().ToUpper();
            }

            Console.Write("Nombre: ");
            string nombre = Console.ReadLine();

            Console.Write("Apellido: ");
            string apellido = Console.ReadLine();

            Console.Write("Fecha de nacimiento (YYYY-MM-DD): ");
            string fechaNac = Console.ReadLine();

            Console.Write("Email: ");
            string email = Console.ReadLine();

            Console.Write("Número de tarjeta (16 dígitos): ");
            string numeroTarjeta = Console.ReadLine();

            // Selección del banco emisor con opciones numeradas
            Console.WriteLine("\nBanco Emisor:");
            Console.WriteLine("  1. Banco Nación");
            Console.WriteLine("  2. Banco Provincia");
            Console.WriteLine("  3. Banco Galicia");
            Console.WriteLine("  4. Banco Santander");
            Console.WriteLine("  5. Banco BBVA");
            Console.WriteLine("  6. Banco Macro");
            Console.Write("Seleccioná el banco (1-6): ");

            string[] bancos = { "Banco Nación", "Banco Provincia", "Banco Galicia", "Banco Santander", "Banco BBVA", "Banco Macro" };
            int opcionBanco;
            while (!int.TryParse(Console.ReadLine(), out opcionBanco) || opcionBanco < 1 || opcionBanco > 6)
            {
                Console.Write("Opción inválida. Ingresá un número del 1 al 6: ");
            }
            string bancoEmisor = bancos[opcionBanco - 1];

            bool exito = EmitirTarjeta(documento, tipoDoc, nombre, apellido, fechaNac, email, numeroTarjeta, bancoEmisor);

            if (exito)
                Console.WriteLine("\n Tarjeta emitida correctamente para " + nombre + " " + apellido + ".");
            else
                Console.WriteLine("\n Error al emitir la tarjeta");

            Console.WriteLine("\nPresione una tecla para volver al menú...");
            Console.ReadKey();
        }

        static void MenuListarTarjetas()
        {
            Console.Clear();
            Console.WriteLine("--- LISTADO GENERAL DE TARJETAS ---");
            Console.WriteLine("{0,-12} {1,-18} {2,-20} {3,-15}", "Nro Cuenta", "Nro Tarjeta", "Banco Emisor", "DNI Titular");
            Console.WriteLine("----------------------------------------------------------------------");

            // === A realizar ===
            // Aquí deben implementar un SELECT sobre la tabla 'tarjetas'
            // para recorrer las filas e imprimirlas en la consola.
            
            ObtenerYMostrarTarjetas();

            Console.WriteLine("\nPresione una tecla para volver al menú...");
            Console.ReadKey();
        }

        static void MenuVerDetalleTarjeta()
        {
            Console.Clear();
            Console.WriteLine("--- DETALLE DE TARJETA Y CLIENTE ---");
            Console.Write("Ingrese el Número de Cuenta a consultar: ");
            int numCuenta = Convert.ToInt32(Console.ReadLine());

            // === A realizar ===
            // Aquí deben realizar un SELECT con un JOIN entre 'tarjetas' y 'usuarios' 
            // filtrando por el numCuenta para traer todos los campos (Nombre, Apellido, Email, Saldo, etc.)
            
            MostrarDetalleCompleto(numCuenta);

            Console.WriteLine("\nPresione una tecla para volver al menú...");
            Console.ReadKey();
        }

        static void MenuEliminarTarjeta()
        {
            Console.Clear();
            Console.WriteLine("--- ELIMINAR TARJETA DEL SISTEMA ---");
            Console.Write("Ingrese el Número de Cuenta de la tarjeta a dar de baja: ");
            int numCuenta = Convert.ToInt32(Console.ReadLine());

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n Se eliminará la tarjeta, sus liquidaciones y los datos de acceso web vinculados.");
            Console.ResetColor();
            Console.Write("¿Está seguro de continuar? (S/N): ");
            
            if (Console.ReadLine().ToUpper() == "S")
            {
                // === A realizar ===
                // Aquí deben ejecutar un DELETE sobre la tabla 'tarjetas' donde num_cuenta = numCuenta.
                // Como definimos ON DELETE CASCADE en la base de datos, las liquidaciones se borrarán solas.
                // Opcional: Evaluar si también eliminan al usuario de la tabla 'usuarios' o si lo mantienen.
                
                bool exito = DarDeBajaTarjeta(numCuenta);

                if (exito)
                    Console.WriteLine("\nTarjeta eliminada correctamente del sistema.");
                else
                    Console.WriteLine("\nError al intentar eliminar la tarjeta. Verifique el número de cuenta.");
            }
            else
            {
                Console.WriteLine("\nOperación cancelada.");
            }

            Console.WriteLine("\nPresione una tecla para volver al menú...");
            Console.ReadKey();
        }

        static void MenuEmitirLiquidacion()
        {
            Console.Clear();
            Console.WriteLine("--- EMITIR NUEVA LIQUIDACIÓN MENSUAL ---\n");

            Console.Write("Número de cuenta: ");
            int numCuenta = Convert.ToInt32(Console.ReadLine());

            Console.Write("Período (YYYY-MM): ");
            string periodo = Console.ReadLine();

            Console.Write("Fecha de vencimiento (YYYY-MM-DD): ");
            string fechaVenc = Console.ReadLine();

            Console.Write("Total a pagar: ");
            decimal totalAPagar = Convert.ToDecimal(Console.ReadLine());

            Console.Write("Pago mínimo: ");
            decimal pagoMinimo = Convert.ToDecimal(Console.ReadLine());

            bool exito = EmitirLiquidacion(numCuenta, periodo, fechaVenc, totalAPagar, pagoMinimo);

            if (exito)
                Console.WriteLine("\nLiquidación emitida correctamente. Ya visible en el portal web.");
            else
                Console.WriteLine("\n Error al emitir la liquidación. Verificá el número de cuenta.");

            Console.WriteLine("\nPresione una tecla para volver al menú...");
            Console.ReadKey();
        }


        // =========================================================================
        // MÉTODOS BASE QUE DEBEN COMPLETAR CON LA LÓGICA 
        // =========================================================================


    static bool EmitirTarjeta(string documento, string tipoDoc, string nombre, string apellido, 
        string fechaNac, string email, string numeroTarjeta, string bancoEmisor)
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(connectionString);
                conn.Open();
                
                MySqlCommand cmdUsuario = new MySqlCommand(
                    "INSERT INTO usuarios (documento, tipo_doc, nombre, apellido, fecha_nacimiento, email) " +
                    "VALUES (@doc, @tipo, @nombre, @apellido, @fecha, @email)", conn);

                cmdUsuario.Parameters.AddWithValue("@doc",      documento);
                cmdUsuario.Parameters.AddWithValue("@tipo",     tipoDoc);
                cmdUsuario.Parameters.AddWithValue("@nombre",   nombre);
                cmdUsuario.Parameters.AddWithValue("@apellido", apellido);
                cmdUsuario.Parameters.AddWithValue("@fecha",    fechaNac);
                cmdUsuario.Parameters.AddWithValue("@email",    email);
                cmdUsuario.ExecuteNonQuery();

                
                MySqlCommand cmdTarjeta = new MySqlCommand(
                    "INSERT INTO tarjetas (numero_tarjeta, banco_emisor, estado, saldo, dni_titular) " +
                    "VALUES (@nro, @banco, 'Activa', 0.00, @doc)", conn);

                cmdTarjeta.Parameters.AddWithValue("@nro",   numeroTarjeta);
                cmdTarjeta.Parameters.AddWithValue("@banco", bancoEmisor);
                cmdTarjeta.Parameters.AddWithValue("@doc",   documento);
                cmdTarjeta.ExecuteNonQuery();

                conn.Close();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nError: " + ex.Message);
                return false;
            }
        }



        static void ObtenerYMostrarTarjetas()
        {
            // Completar 
            // Ejemplo de impresión dentro del bucle: 
            // Console.WriteLine("{0,-12} {1,-18} {2,-20} {3,-15}", reader["num_cuenta"], reader["numero_tarjeta"], ...);
        try
            {
                MySqlConnection conn = new MySqlConnection(connectionString);
                conn.Open();

                MySqlCommand    cmd    = new MySqlCommand("SELECT num_cuenta, numero_tarjeta, banco_emisor, dni_titular FROM tarjetas", conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Console.WriteLine("{0,-12} {1,-20} {2,-20} {3,-15}",
                        reader["num_cuenta"],
                        reader["numero_tarjeta"],
                        reader["banco_emisor"],
                        reader["dni_titular"]);
                }

                reader.Close();
                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al listar tarjetas: " + ex.Message);
            }
        
        }

        static void MostrarDetalleCompleto(int cuenta)
        {
            // Completar haciendo un SELECT con JOIN de usuarios y tarjetas WHERE num_cuenta = @cuenta
            try
            {
                MySqlConnection conn = new MySqlConnection(connectionString);
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(
                    "SELECT u.nombre, u.apellido, u.email, u.documento, " +
                    "       t.num_cuenta, t.numero_tarjeta, t.banco_emisor, t.estado, t.saldo " +
                    "FROM tarjetas t " +
                    "JOIN usuarios u ON u.documento = t.dni_titular " +
                    "WHERE t.num_cuenta = @cuenta", conn);

                cmd.Parameters.AddWithValue("@cuenta", cuenta);

                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    Console.WriteLine("\n── DATOS DEL CLIENTE ──────────────────────");
                    Console.WriteLine("Nombre:      " + reader["nombre"] + " " + reader["apellido"]);
                    Console.WriteLine("Documento:   " + reader["documento"]);
                    Console.WriteLine("Email:       " + reader["email"]);
                    Console.WriteLine("\n── DATOS DE LA TARJETA ─────────────────────");
                    Console.WriteLine("Nro Cuenta:  " + reader["num_cuenta"]);
                    Console.WriteLine("Nro Tarjeta: " + reader["numero_tarjeta"]);
                    Console.WriteLine("Banco:       " + reader["banco_emisor"]);
                    Console.WriteLine("Estado:      " + reader["estado"]);
                    Console.WriteLine("Saldo:       $" + reader["saldo"]);
                }
                else
                {
                    Console.WriteLine("\nNo se encontró ninguna tarjeta con ese número de cuenta.");
                }

                reader.Close();
                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener detalle: " + ex.Message);
            }
        }

        static bool DarDeBajaTarjeta(int cuenta)
        {
            // Completar usando un DELETE FROM tarjetas WHERE num_cuenta = @cuenta  
            try
            {
                MySqlConnection conn = new MySqlConnection(connectionString);
                conn.Open();

                // Al borrar la tarjeta, las liquidaciones se borran solas por ON DELETE CASCADE
                MySqlCommand cmd = new MySqlCommand(
                    "DELETE FROM tarjetas WHERE num_cuenta = @cuenta", conn);

                cmd.Parameters.AddWithValue("@cuenta", cuenta);

                int filasAfectadas = cmd.ExecuteNonQuery();

                conn.Close();
                return filasAfectadas > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al eliminar: " + ex.Message);
                return false;
            }
        }
static bool EmitirLiquidacion(int numCuenta, string periodo, string fechaVenc, decimal totalAPagar, decimal pagoMinimo)
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(connectionString);
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(
                    "INSERT INTO liquidaciones (num_cuenta, periodo, fecha_vencimiento, total_a_pagar, pago_minimo) " +
                    "VALUES (@cuenta, @periodo, @venc, @total, @minimo)", conn);

                cmd.Parameters.AddWithValue("@cuenta",  numCuenta);
                cmd.Parameters.AddWithValue("@periodo", periodo);
                cmd.Parameters.AddWithValue("@venc",    fechaVenc);
                cmd.Parameters.AddWithValue("@total",   totalAPagar);
                cmd.Parameters.AddWithValue("@minimo",  pagoMinimo);

                int filasAfectadas = cmd.ExecuteNonQuery();

                conn.Close();
                return filasAfectadas > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al emitir liquidación: " + ex.Message);
                return false;
            }
        }
    }
}