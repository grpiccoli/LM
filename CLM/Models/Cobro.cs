using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CLM.Models
{
    public class Cobro
    {
        public int Id { get; set;}

        public int ValeId { get; set; }
        public virtual Vale Vale { get; set; }

        public Honorario Honorario { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public int Monto { get; set; }
    }
    public enum Honorario
    {
        [Display(Name = "Saldo inicial", GroupName = "Saldo Inicial")]
        SaldoInicial,
        [Display(Name = "Mensuales", GroupName ="Honorarios")]
        Mensuales,
        [Display(Name = "Laborales", GroupName = "Honorarios")]
        Laborales,
        [Display(Name = "Renta", GroupName = "Honorarios")]
        Renta,
        [Display(Name = "Retención", GroupName = "Honorarios")]
        Retención,
        [Display(Name = "Impuestos renta", GroupName = "SII")]
        Impuestosrenta,
        [Display(Name = "Multas varias", GroupName = "SII")]
        Multasvarias,
        [Display(Name = "Cambio de sujeto", GroupName = "SII")]
        Cambiodesujeto,
        [Display(Name = "Inicio de actividades", GroupName = "SII")]
        Iniciodeactividades,
        [Display(Name = "Modificación de inicio de actividades", GroupName = "SII")]
        Modificacióndeiniciodeactividades,
        [Display(Name = "Timbrajes", GroupName = "SII")]
        Timbrajes,
        [Display(Name = "Certificados digitales", GroupName = "SII")]
        Certificadosdigitales,
        [Display(Name = "Termino de giro", GroupName = "SII")]
        Terminodegiro,
        [Display(Name = "Carpetas tributarias", GroupName = "SII")]
        Carpetastributarias,
        [Display(Name = "Contribuciones bienes raíces", GroupName = "SII")]
        Contribucionesbienesraíces,
        [Display(Name = "Declaraciones juradas", GroupName = "SII")]
        Declaracionesjuradas,
        [Display(Name = "DJ 1887", GroupName = "SII")]
        DJ1887,
        [Display(Name = "Aviso venta de vehículo", GroupName = "SII")]
        Avisoventadevehículo,
        [Display(Name = "Impuestos Mensuales F29", GroupName = "SII")]
        ImpuestosMensualesF29,
        [Display(Name = "Contratos", GroupName = "Laboral")]
        Contratos,
        [Display(Name = "Cláusulas anexas", GroupName = "Laboral")]
        Cláusulasanexas,
        [Display(Name = "Certificado de vacaciones", GroupName = "Laboral")]
        Certificadodevacaciones,
        [Display(Name = "Certificado de antigüedad", GroupName = "Laboral")]
        Certificadodeantigüedad,
        [Display(Name = "Finiquitos", GroupName = "Laboral")]
        Finiquitos,
        [Display(Name = "Constancias", GroupName = "Laboral")]
        Constancias,
        [Display(Name = "Representación AFP", GroupName = "Laboral")]
        RepresentaciónAFP,
        [Display(Name = "Libro de remuneraciones", GroupName = "Laboral")]
        Libroderemuneraciones,
        [Display(Name = "Libro de asistencia", GroupName = "Laboral")]
        Librodeasistencia,
        [Display(Name = "Carta de recomendación", GroupName = "Laboral")]
        Cartaderecomendación,
        [Display(Name = "Carta aviso de despido", GroupName = "Laboral")]
        Cartaavisodedespido,
        [Display(Name = "Carta renuncia voluntaria", GroupName = "Laboral")]
        Cartarenunciavoluntaria,
        [Display(Name = "Certificado por permiso de matrimonio", GroupName = "Laboral")]
        Certificadoporpermisodematrimonio,
        [Display(Name = "Certificado por nacimiento de hijo o hija", GroupName = "Laboral")]
        Certificadopornacimientodehijoohija,
        [Display(Name = "Certificado por defunción familiar directo", GroupName = "Laboral")]
        Certificadopordefunciónfamiliardirecto,
        [Display(Name = "Licencias médicas", GroupName = "Laboral")]
        Licenciasmédicas,
        [Display(Name = "Certificado de ingresos", GroupName = "Laboral")]
        Certificadodeingresos,
        [Display(Name = "Trámite de asignación familiar", GroupName = "Laboral")]
        Trámitedeasignaciónfamiliar,
        [Display(Name = "Trámite de FONASA", GroupName = "Laboral")]
        TrámitedeFONASA,
        [Display(Name = "Trámite de ISAPRE", GroupName = "Laboral")]
        TrámitedeISAPRE,
        [Display(Name = "Pago de planillas atrasadas", GroupName = "Laboral")]
        Pagodeplanillasatrasadas,
        [Display(Name = "Confección de reglamentos internos", GroupName = "Laboral")]
        Confeccióndereglamentosinternos,
        [Display(Name = "Derecho a saber", GroupName = "Laboral")]
        Derechoasaber,
        [Display(Name = "Entrega de elementos de protección", GroupName = "Laboral")]
        Entregadeelementosdeprotección,
        [Display(Name = "Archivador", GroupName = "Varios")]
        Archivador,
        [Display(Name = "Huellador", GroupName = "Varios")]
        Huellador,
        [Display(Name = "Libros contables", GroupName = "Varios")]
        Libroscontables,
        [Display(Name = "Certificados de ingresos", GroupName = "Varios")]
        Certificadosdeingresos,
        [Display(Name = "Confección de documentos tributarios", GroupName = "Varios")]
        Confeccióndedocumentostributarios,
        [Display(Name = "Envío de correspondencia", GroupName = "Varios")]
        Envíodecorrespondencia,
        [Display(Name = "Sanidad", GroupName = "Varios")]
        Sanidad,
        [Display(Name = "Patentes comerciales", GroupName = "Varios")]
        Patentescomerciales,
        [Display(Name = "Posesiones efectivas", GroupName = "Contable")]
        Posesionesefectivas,
        [Display(Name = "Constitución de empresas", GroupName = "Contable")]
        Constitucióndeempresas,
        [Display(Name = "Poderes", GroupName = "Contable")]
        Poderes,
        [Display(Name = "Charlas y capacitaciones", GroupName = "Contable")]
        Charlasycapacitaciones,
        [Display(Name = "2500 Boletas", GroupName = "Imprenta")]
        Boletas2500,
        [Display(Name = "5000 Boletas", GroupName = "Imprenta")]
        Boletas5000,
        [Display(Name = "500 T64", GroupName = "Imprenta")]
        T64x500,
        [Display(Name = "10000 Boletas", GroupName = "Imprenta")]
        Boletas10000,
        [Display(Name = "1000 Tamaño 64", GroupName = "Imprenta")]
        Tamaño64x1000,
        [Display(Name = "Boletas Mayor Cantidad", GroupName = "Imprenta")]
        BoletasMayorCantidad,
        [Display(Name = "25 Facturas", GroupName = "Imprenta")]
        Facturas25,
        [Display(Name = "50 Facturas", GroupName = "Imprenta")]
        Facturas50,
        [Display(Name = "25 Facturas Exentas", GroupName = "Imprenta")]
        FacturasExentas25,
        [Display(Name = "50 Facturas Exentas", GroupName = "Imprenta")]
        FacturasExentas50,
        [Display(Name = "25 Guías de despacho", GroupName = "Imprenta")]
        Guíasdedespacho25,
        [Display(Name = "50 Guías de despacho", GroupName = "Imprenta")]
        Guíasdedespacho50,
        [Display(Name = "25 Guías de despacho exentas", GroupName = "Imprenta")]
        Guíasdedespachoexentas25,
        [Display(Name = "50 Guías de despacho exentas", GroupName = "Imprenta")]
        Guíasdedespachoexentas50,
        [Display(Name = "25 Facturas de compra", GroupName = "Imprenta")]
        Facturasdecompra25,
        [Display(Name = "50 Facturas de compra", GroupName = "Imprenta")]
        Facturasdecompra50,
        [Display(Name = "F30", GroupName = "Inspección del Trabajo")]
        F30,
        [Display(Name = " F30-1", GroupName = "Inspección del Trabajo")]
        F30_1,
    }
}
