namespace VeterisDeviceServer.SSL
{
    /// <summary>
    /// Detalles sobre un certificado
    /// </summary>
    public class CertificateSubject
    {
        /// <summary>
        /// Organización
        /// </summary>
        public string Organization { get; set; }

        /// <summary>
        /// Unidad de Organización
        /// </summary>
        public string OrganizationUnit { get; set; }

        /// <summary>
        /// País
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Estado o provincia
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Localidad
        /// </summary>
        public string Locality { get; set; }

        /// <summary>
        /// Nombre común o nombre de dominio
        /// </summary>
        public string CommonName { get; set; }

        /// <summary>
        /// Serializar sujeto de certificado
        /// </summary>
        public string Serialize()
        {
            return $"/O={Organization}/OU={OrganizationUnit}/C={Country}/ST={State}/L={Locality}/CN={CommonName}";
        }
    }
}
