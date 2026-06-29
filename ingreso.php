<?php
session_start();

$server   = "localhost";
$usuario  = "root";
$password = "root";

$conexion = new mysqli($server, $usuario, $password, "mi_banco_db");

if ($conexion->connect_error) {
    die("Error al conectar: " . $conexion->connect_error);
}

$tipo_doc  = $_POST["tipo_doc"];
$documento = $_POST["documento"];
$usuario   = $_POST["usuario"];
$password  = $_POST["password"];

$sql    = "SELECT * FROM usuarios WHERE usuario='$usuario' AND tipo_doc='$tipo_doc' AND documento='$documento'";
$result = $conexion->query($sql);

if ($result && $result->num_rows > 0) {
    $row = $result->fetch_assoc();
    if ($password === $row['password']) {          // texto plano, como pide el TP
        $_SESSION["documento"] = $row["documento"];
        $_SESSION["nombre"]    = $row["nombre"];
        header("Location: resumen.php");
        exit();
    } else {
        die("Usuario o contraseña incorrectos.");
    }
} else {
    die("Usuario o contraseña incorrectos.");
}

$conexion->close();
?>