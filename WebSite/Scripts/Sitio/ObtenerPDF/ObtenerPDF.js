// Función para manejar el evento drag and drop
function configurarDragAndDrop(dragAreaId, fileInputId) {
    const dragArea = document.querySelector(dragAreaId);
    const fileInput = document.querySelector(fileInputId);

    // Evento cuando el área está sobre el archivo
    dragArea.addEventListener('dragover', (e) => {
        e.preventDefault();
        dragArea.style.backgroundColor = '#f0f8ff'; // Cambiar color de fondo al estar sobre el área
    });

    // Evento cuando el archivo sale del área
    dragArea.addEventListener('dragleave', () => {
        dragArea.style.backgroundColor = ''; // Restaurar el color de fondo
    });

    // Evento cuando el archivo es soltado
    dragArea.addEventListener('drop', (e) => {
        e.preventDefault();
        const file = e.dataTransfer.files[0];
        fileInput.files = e.dataTransfer.files;
        dragArea.querySelector('p').textContent = file.name;
        dragArea.style.backgroundColor = ''; // Restaurar color después de soltar el archivo
    });
}

// Función para actualizar el nombre del archivo cuando se selecciona desde el explorador
function updateFileName(fileInputId, dragAreaId, uuidDisplayId, WSUrl) {
    const fileInput = document.querySelector(fileInputId);
    const fileName = fileInput.files[0].name;
    const dragArea = document.querySelector(dragAreaId);
    dragArea.querySelector('p').textContent = fileName;
    processXMLFile(fileInputId, uuidDisplayId, WSUrl);
}

// Función para leer el archivo XML y extraer el UUID
function processXMLFile(fileInputId, uuidDisplayId, WSUrl) {
    const fileInput = document.querySelector(fileInputId);
    const file = fileInput.files[0];
    let uuid = '';
    
    if (file) {
        if (file.name.endsWith('.xml')) {
            const reader = new FileReader();
            reader.onload = function (e) {
                const parser = new DOMParser();
                const xmlDoc = parser.parseFromString(e.target.result, "application/xml");
                
                // Buscar el UUID de manera dinámica
                $(xmlDoc).find('*').each(function () {
                    const uuidAttr = $(this).attr('UUID');
                    if (uuidAttr) {
                        uuid = uuidAttr;
                        return false; // Detener el ciclo una vez que encontramos el UUID
                    }
                });

                // Mostrar el UUID en el HTML
                if (uuid) {
                    $(uuidDisplayId).text(uuid);
                    const usuario = 'demo1@mail.com'; // Puede ser dinámico
                    const password = 'Demo123#'; // Puede ser dinámico

                    // Generar la solicitud SOAP con los parámetros
                    const soapRequest = generateSOAPRequest(usuario, password, uuid);

                    // Llamada AJAX al servicio web SOAP
                    callSOAPService(soapRequest, WSUrl);
                } else {
                    $(uuidDisplayId).text('UUID no encontrado.');
                    alert('El XML no tiene los datos necesarios.');
                }
            };
            reader.readAsText(file);
        } else {
            alert('Por favor selecciona un archivo XML válido.');
        }
    } else {
        alert('No se seleccionó ningún archivo.');
    }
}

// Función para generar la solicitud SOAP
function generateSOAPRequest(usuario, password, uuid) {
    return SoapService.ObtenerSolicitudSOAP(usuario, password, uuid);
}

// Función para hacer la llamada AJAX al servicio web
function callSOAPService(soapRequest, WSUrl) {
    const consumUrl = 'https://cors-anywhere.herokuapp.com/' + WSUrl;

    $.ajax({
        url: consumUrl,  
        type: 'POST',    
        contentType: 'text/xml; charset="utf-8"',
        data: soapRequest,
        success: function (response) {

            // Convertimos el XML a texto
            const xmlString = new XMLSerializer().serializeToString(response);

            // Parseamos el XML
            const parser = new DOMParser();
            const xmlDoc = parser.parseFromString(xmlString, 'text/xml');

            // Verificamos si hubo algún error en el parseo
            if (xmlDoc.getElementsByTagName('parsererror').length > 0) {
                console.error('Error al parsear el XML');
                return;
            }

            // Extraemos la cadena Base64 del PDF
            const pdfBase64 = xmlDoc.getElementsByTagName('PDF')[0]?.textContent;

            if (pdfBase64) {
                // Mostrar el PDF en un iframe
                const pdfWindow = window.open();
                pdfWindow.document.write('<iframe width="100%" height="100%" src="data:application/pdf;base64,' + pdfBase64 + '"></iframe>');
            } else {
                alert('No se encontró el PDF en la respuesta.');
            }
        },
        error: function (xhr, status, error) {
            alert('Error al hacer la llamada SOAP:', error);
        }
    });


}



