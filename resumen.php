<?php
session_start();


if (!isset($_SESSION["documento"])) {
    header("Location: ingreso.html");
    exit();
}

$server   = "localhost";
$db_user  = "root";
$db_pass  = "root";
$conexion = new mysqli($server, $db_user, $db_pass, "mi_banco_db");

if ($conexion->connect_error) {
    die("Error al conectar: " . $conexion->connect_error);
}

$documento = $_SESSION["documento"];

// ── Query 1: datos del usuario + tarjeta (JOIN) ──
$sql_tarjeta = "SELECT u.nombre, u.apellido, u.email,
                t.num_cuenta, t.numero_tarjeta, t.banco_emisor, t.estado, t.saldo
                FROM usuarios u
                JOIN tarjetas t ON t.dni_titular = u.documento
                WHERE u.documento = '$documento'";

$res_tarjeta = $conexion->query($sql_tarjeta);
$tarjeta     = $res_tarjeta->fetch_assoc();    

// ── Query 2: última liquidación ──
$sql_ultima = "SELECT * FROM liquidaciones WHERE num_cuenta = '{$tarjeta['num_cuenta']}'ORDER BY periodo DESC
        LIMIT 1";

$res_ultima = $conexion->query($sql_ultima);
$ultima     = $res_ultima->fetch_assoc();

// ── Query 3: historial (todo menos la última) ──
$sql_historial = "SELECT * FROM liquidaciones
                WHERE num_cuenta = '{$tarjeta['num_cuenta']}' ORDER BY periodo DESC
                LIMIT 100 OFFSET 1";

$res_historial = $conexion->query($sql_historial);

$conexion->close();
?>
<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Mis Tarjetas - Mi Resumen</title>
    <script src="https://cdn.tailwindcss.com"></script>
</head>
<body class="bg-gray-100 font-sans min-h-screen flex flex-col">


    <header class="bg-[#004691] text-white py-4 shadow-md">
        <div class="max-w-5xl mx-auto px-6 flex justify-between items-center">
            <h1 class="text-xl font-semibold">Mis <span class="font-bold">Tarjetas</span></h1>
            <div class="flex items-center gap-4 text-sm">
                <span>Hola, <strong><?= htmlspecialchars($tarjeta['nombre']) ?></strong></span>
                <a href="cerrar_sesion.php" class="bg-white text-[#004691] font-semibold px-4 py-1 rounded-full hover:bg-gray-100 transition">
                    Cerrar sesión
                </a>
            </div>
        </div>
    </header>

    <main class="flex-grow max-w-5xl mx-auto w-full px-6 py-8 space-y-8">


        <section class="bg-white rounded-xl shadow p-6">
            <h2 class="text-xs font-semibold text-gray-400 uppercase tracking-widest mb-4">Tu tarjeta</h2>
            <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <div>
                    <p class="text-xs text-gray-400 uppercase">Titular</p>
                    <p class="font-semibold text-gray-800">
                        <?= htmlspecialchars($tarjeta['nombre'] . ' ' . $tarjeta['apellido']) ?>
                    </p>
                </div>
                <div>
                    <p class="text-xs text-gray-400 uppercase">Email</p>
                    <p class="font-semibold text-gray-800"><?= htmlspecialchars($tarjeta['email']) ?></p>
                </div>
                <div>
                    <p class="text-xs text-gray-400 uppercase">Número de tarjeta</p>
                    <p class="font-semibold text-gray-800 tracking-widest">
                        **** **** **** <?= substr($tarjeta['numero_tarjeta'], -4) ?>
                    </p>
                </div>
                <div>
                    <p class="text-xs text-gray-400 uppercase">Banco emisor</p>
                    <p class="font-semibold text-gray-800"><?= htmlspecialchars($tarjeta['banco_emisor']) ?></p>
                </div>
                <div>
                    <p class="text-xs text-gray-400 uppercase">Estado</p>
                    <span class="inline-block px-3 py-1 rounded-full text-xs font-bold
                        <?= $tarjeta['estado'] === 'Activa' ? 'bg-green-100 text-green-700' : 'bg-red-100 text-red-700' ?>">
                        <?= $tarjeta['estado'] ?>
                    </span>
                </div>
                <div>
                    <p class="text-xs text-gray-400 uppercase">Saldo actual</p>
                    <p class="font-bold text-2xl text-[#004691]">
                        $<?= number_format($tarjeta['saldo'], 2, ',', '.') ?>
                    </p>
                </div>
            </div>
        </section>


        <?php if ($ultima): ?>
        <section class="bg-[#004691] text-white rounded-xl shadow p-6">
            <h2 class="text-xs font-semibold text-blue-200 uppercase tracking-widest mb-4">
                Última liquidación — Período <?= htmlspecialchars($ultima['periodo']) ?>
            </h2>
            <div class="grid grid-cols-1 sm:grid-cols-3 gap-6">
                <div>
                    <p class="text-blue-200 text-xs uppercase">Vencimiento</p>
                    <p class="text-xl font-bold"><?= htmlspecialchars($ultima['fecha_vencimiento']) ?></p>
                </div>
                <div>
                    <p class="text-blue-200 text-xs uppercase">Total a pagar</p>
                    <p class="text-3xl font-bold">$<?= number_format($ultima['total_a_pagar'], 2, ',', '.') ?></p>
                </div>
                <div>
                    <p class="text-blue-200 text-xs uppercase">Pago mínimo</p>
                    <p class="text-xl font-bold">$<?= number_format($ultima['pago_minimo'], 2, ',', '.') ?></p>
                </div>
            </div>
        </section>
        <?php else: ?>
        <section class="bg-white rounded-xl shadow p-6 text-center text-gray-400">
            <p>No hay liquidaciones emitidas todavía.</p>
        </section>
        <?php endif; ?>

        <?php if ($res_historial && $res_historial->num_rows > 0): ?>
        <section class="bg-white rounded-xl shadow p-6">
            <h2 class="text-xs font-semibold text-gray-400 uppercase tracking-widest mb-4">Historial de liquidaciones</h2>
            <div class="overflow-x-auto">
                <table class="w-full text-sm text-left">
                    <thead>
                        <tr class="text-xs text-gray-400 uppercase border-b border-gray-200">
                            <th class="pb-2 pr-4">Período</th>
                            <th class="pb-2 pr-4">Vencimiento</th>
                            <th class="pb-2 pr-4">Total a pagar</th>
                            <th class="pb-2">Pago mínimo</th>
                        </tr>
                    </thead>
                    <tbody class="divide-y divide-gray-100">
                        <?php while ($fila = $res_historial->fetch_assoc()): ?>
                        <tr class="text-gray-700 hover:bg-gray-50">
                            <td class="py-3 pr-4 font-medium"><?= htmlspecialchars($fila['periodo']) ?></td>
                            <td class="py-3 pr-4"><?= htmlspecialchars($fila['fecha_vencimiento']) ?></td>
                            <td class="py-3 pr-4">$<?= number_format($fila['total_a_pagar'], 2, ',', '.') ?></td>
                            <td class="py-3">$<?= number_format($fila['pago_minimo'], 2, ',', '.') ?></td>
                        </tr>
                        <?php endwhile; ?>
                    </tbody>
                </table>
            </div>
        </section>
        <?php endif; ?>

    </main>

    <footer class="bg-gray-50 text-[10px] text-gray-400 text-center p-4 border-t border-gray-200">
        Portal Oficial de Consultas de Liquidaciones Progra3card.
    </footer>

</body>
</html>