using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;
using ClarityServices.Database;
using ClarityServices.System;

namespace LedorVadorASMX {
    /// <summary>
    /// Summary description for LVService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class LVService : System.Web.Services.WebService {

        public static Logger logger = new Logger(ConfigurationManager.AppSettings.Get("SERVICE_LOG_PATH"));


        public class LedorVadorResponse {
            public List<int> documentosAceptados = new List<int>();
            public List<int> documentosOmitidos = new List<int>();
            public string responseMessage = "";
        }
        public class LedorVadorRequest {
            public string login = "";
            public string password = "";
            public List<LedorVadorFlowPackage> packages = new List<LedorVadorFlowPackage>();
        }
        public class LedorVadorFlowPackage {
            public int histClinica;
            public string razonSocial;
            public int empresaPropia;
            public string documento_externId;
            public string establecimiento_numero;
            public string documento_numero;
            public DateTime fecha;
            public string observacion;
            public int tipoServicioId;
            public int tipoImpuestoId_Iva;
            public double cantidad;
            public double importeUnitario;
        }


        [WebMethod]
        public LedorVadorResponse SendPackages(LedorVadorRequest request) {
            LedorVadorResponse response = new LedorVadorResponse();
            int aceptados = 0, omitidos = 0;
            string warningMsg = "", exceptionMsg = "";
            //
            DbHandler db = new DbHandler(ConfigurationManager.ConnectionStrings["db"].ConnectionString);
            request.login = ConfigurationManager.AppSettings.Get("LOGIN");
            request.password = ConfigurationManager.AppSettings.Get("PASS");
            db.Using(request.login, request.password, (SqlConnection conn) => {
                //
                foreach (LedorVadorFlowPackage package in request.packages) {
                    try {
                        logger.write("Leyendo documento " + package.documento_externId + " cliente " + package.histClinica + " " + package.razonSocial);
                        string localWarningMsg = "";
                        localWarningMsg = CheckPackage(package);
                        if (localWarningMsg == "") { //Si se genero un mensaje de warning se omite directamente el paquete.
                            localWarningMsg = savePackage(db, conn, package);
                            if (localWarningMsg == "") {
                                try {
                                    response.documentosAceptados.Add(Int32.Parse(package.documento_externId));
                                } catch (Exception e) {
                                    localWarningMsg = "[" + e.Message + "]";
                                }
                            }
                        }
                        warningMsg += localWarningMsg;
                        aceptados++;
                    } catch (Exception e) {
                        try {
                            response.documentosOmitidos.Add(Int32.Parse(package.documento_externId));
                        } catch (Exception ex) {
                            warningMsg += "[" + ex.Message + "]";
                        }
                        logger.write(e.ToString());
                        exceptionMsg += Environment.NewLine + e.ToString().Substring(0, 300);
                        omitidos++;
                    }
                }
                //
            }, (string exMessage) => {
                exceptionMsg += Environment.NewLine + exMessage.Substring(0, 300);
            });
            //
            response.responseMessage = " Aceptados [" + aceptados + "]";
            response.responseMessage += " Omitidos [" + omitidos + "]";
            if (warningMsg != "") {
                response.responseMessage += " Warning [" + warningMsg + "]";
            }
            if (exceptionMsg != "") {
                response.responseMessage += Environment.NewLine + " ServerExceptions [" + exceptionMsg + "]";
            }
            return response;
        }

        private string savePackage(DbHandler db, SqlConnection conn, LedorVadorFlowPackage package) {
            try {
                DataSet ds = db.Call("MIG_DocumentoFromQTECH_sp", conn, (SqlDataAdapter da) => {
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    da.SelectCommand.Parameters.AddWithValue("@HistClinica", DbType.Int32).Value = package.histClinica;
                    da.SelectCommand.Parameters.AddWithValue("@RazonSocial", DbType.String).Value = package.razonSocial;
                    da.SelectCommand.Parameters.AddWithValue("@EmpresaID_Propia", DbType.Int32).Value = package.empresaPropia;
                    da.SelectCommand.Parameters.AddWithValue("@Documento_ExternID", DbType.String).Value = package.documento_externId;
                    da.SelectCommand.Parameters.AddWithValue("@Establecimiento_Numero", DbType.String).Value = package.establecimiento_numero;
                    da.SelectCommand.Parameters.AddWithValue("@Documento_Numero", DbType.String).Value = package.documento_numero;
                    da.SelectCommand.Parameters.AddWithValue("@Fecha", DbType.DateTime).Value = package.fecha;
                    da.SelectCommand.Parameters.AddWithValue("@Observacion", DbType.String).Value = package.observacion;
                    da.SelectCommand.Parameters.AddWithValue("@TipoServicioID", DbType.Int32).Value = package.tipoServicioId;
                    da.SelectCommand.Parameters.AddWithValue("@TipoImpuestoID_IVA", DbType.Int32).Value = package.tipoImpuestoId_Iva;
                    da.SelectCommand.Parameters.AddWithValue("@Cantidad", DbType.Decimal).Value = package.cantidad;
                    da.SelectCommand.Parameters.AddWithValue("@ImporteUnitario", DbType.Double).Value = package.importeUnitario;
                });
                try {
                    string posibleError = ds.Tables[0].Rows[0].ToString();
                    return posibleError;
                } catch (Exception) {
                    return "";
                }
            } catch (Exception e) {
                return e.Message;
            }
        }




        private string CheckPackage(LedorVadorFlowPackage package) {
            string msg = "";
            if (package.histClinica.ToString().Length < 2) msg += "-histClinica";
            if (package.establecimiento_numero == "") msg += "-establecimiento_numero";
            if (package.documento_numero == "") msg += "-documento_numero";
            if (package.razonSocial == "") msg += "-razonSocial";
            if (package.documento_externId == "") msg += "-documento_externId";
            //
            string id = " | " +
                ((package.documento_externId == null || package.documento_externId == "") ? "unknow"
                : package.documento_externId);
            //
            if (msg != "") {
                return id + " requerido-> " + msg;
            }
            return "";
        }
    }
}
