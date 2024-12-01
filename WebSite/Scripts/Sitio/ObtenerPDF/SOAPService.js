class SoapService {
    // M�todo para construir el XML con los par�metros proporcionados
    static ObtenerSolicitudSOAP(usuario, password, uuid) {
        // Usamos template literals para crear el XML din�micamente
        return `<?xml version="1.0" encoding="UTF-8"?>
        <soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/"
                          xmlns:web="http://WebService/">
            <soapenv:Header/>
            <soapenv:Body>
                <web:ObtenerPDF>
                    <Usuario>${usuario || ''}</Usuario>
                    <Password>${password || ''}</Password>
                    <uuid>${uuid || ''}</uuid>
                </web:ObtenerPDF>
            </soapenv:Body>
        </soapenv:Envelope>`;
    }
}
