<?php
$server   = "localhost";
$usuario  = "root";
$password = "root";

$conexion = new mysqli($server, $usuario, $password, "mi_banco_db");

if ($conexion->connect_error) {
    die("Error al conectar: " . $conexion->connect_error);
}

$tipo_doc         = $_POST["tipo_doc"];
$documento        = $_POST["documento"];
$nombre           = $_POST["nombre"];
$apellido         = $_POST["apellido"];
$fecha_nacimiento = $_POST["fecha_nacimiento"];
$email            = $_POST["email"];  
$usuario_form     = $_POST["usuario"];
$passwordA        = $_POST["passwordA"];
$passwordB        = $_POST["passwordB"];

if ($passwordA !== $passwordB) {
    die("Error: las contraseñas no coinciden.");
}

// El TP pide UPDATE porque el usuario ya existe (lo cargó C#)
// Solo activamos su cuenta web seteando usuario y password
$sql = "UPDATE usuarios 
        SET usuario='$usuario_form', password='$passwordA'
        WHERE documento='$documento' AND tipo_doc='$tipo_doc'
    AND nombre='$nombre' AND apellido='$apellido'";

if (!$conexion->query($sql) || $conexion->affected_rows === 0) {
    die("Error: no se encontró una cuenta asociada a ese documento. Verificá tus datos.");
}

$conexion->close();
?>

<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8">
    <title>Activación exitosa</title>
</head>
<body>
    <h1>¡Cuenta activada!</h1>
    <p>Tu usuario web fue creado con éxito.</p>
    <a href="ingreso.html">Iniciar sesión</a>
</body>
</html>