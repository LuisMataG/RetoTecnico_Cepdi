//Consumo de API GET con paginación.
function cargarMedicamentos(page, pageSize, baseUrl) {
    $.ajax({
        url: baseUrl + 'api/Medicamentos',  // URL para obtener los medicamentos
        type: 'GET',
        data: { page: page, pageSize: pageSize },
        success: function (response) {
            if (response.success) {
                cargarTabla(response);  // Cargar los medicamentos en la tabla si la respuesta es exitosa
            } else {
                console.log('Error al obtener los medicamentos:', response.data);
            }
        },
        error: function (xhr, status, error) {
            console.log('Error en la solicitud:', error);
        }
    });
}
//Función para crear la tabla medicamentos
function cargarTabla(response) {
    $('#meciamentosTable tbody').empty();
    $('#pagination').empty();

    response.medicamentos.forEach(function (medicamento) {
        var precioFormateado = medicamento.precio.toLocaleString('es-MX', {
            style: 'currency',
            currency: 'MXN',
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
        });
        var row = `<tr data-id="${medicamento.idmedicamento}">
                <td><input type="checkbox" class="medicamento-checkbox" data-id="${medicamento.idmedicamento}" aria-label="Seleccionar medicamento"></td>
                <td>${medicamento.nombre}</td>
                <td>${medicamento.concentracion}</td>
                <td>${precioFormateado}</td>
                <td>${medicamento.stock}</td>
                <td>${medicamento.presentacion}</td>
                <td>${medicamento.formaFarmaceuticaNombre}</td>
            </tr>`;
        $('#meciamentosTable tbody').append(row);
    });

    // Crear botones de paginación después de cargar la tabla
    crearPaginacion(response.paginaActual, response.totalPaginas);
}
// Función para crear los botones de paginación
function crearPaginacion(paginaActual, totalPaginas) {
    var paginationHTML = '';

    // Crear un botón para la página anterior
    if (paginaActual > 1) {
        paginationHTML += `<button class="pagination-btn" data-page="${paginaActual - 1}"><</button>`;
    }

    // Crear botones para cada página
    for (var i = 1; i <= totalPaginas; i++) {
        if (i === paginaActual) {
            paginationHTML += `<button class="pagination-btn active" data-page="${i}">${i}</button>`;
        } else {
            paginationHTML += `<button class="pagination-btn" data-page="${i}">${i}</button>`;
        }
    }

    // Crear un botón para la página siguiente
    if (paginaActual < totalPaginas) {
        paginationHTML += `<button class="pagination-btn" data-page="${paginaActual + 1}">></button>`;
    }

    // Insertar los botones de paginación en el contenedor
    $('#pagination').append(paginationHTML);
}

// Función para manejar el clic en los botones de paginación
function cambiarDePagina(pageSize, baseUrl) {
    $(document).on('click', '.pagination-btn', function () {
        var page = $(this).data('page');  // Obtener el número de página seleccionado
        console.log('Page: ' + page + '. Tamaño: ' + pageSize);
        cargarMedicamentos(page, pageSize, baseUrl);  // Obtener los medicamentos para la página seleccionada
    });
}

//Función utilizada para que al dar clic sobre cualquier parte de la fila se muestre el modal con los datos del medicamento.
function mostrarModalEdicion(urlModal) {
    $(document).on('click', 'tr td:not(:first-child)', function () {
        var medicamentoId = $(this).closest('tr').data('id'); // Obtener el ID del medicamento de la fila
        console.log(medicamentoId);
        // Hacer una solicitud para obtener el formulario de edición

        console.log(urlModal);
        $.get(urlModal, { id: medicamentoId }, function (data) {
            console.log('Se ejecuto2');
            // Inyectar el formulario cargado dentro del modal
            $('#modalContent').html(data);
            // Mostrar el modal
            $('#editModal').modal('show');
        });
    });
}

//Función para desplegar modal al agregar nuevo medicamento
function agregarNuevoMedicamento(urlEditModal) {
    $(document).on('click', '#addMedic', function () {
        $.get(urlEditModal, { id: 0 }, function (data) {
            $('#modalContent').html(data);
            $('#editModal').modal('show');
        });
    });
}

//Función para el seleccionar todos los checkbox y mostrar el boton eliminar.
function seleccionarTodosChkBox() {
    $('#selectAll').change(function () {
        // Cambiar el estado de todos los checkboxes de la tabla
        $('.medicamento-checkbox').prop('checked', $(this).prop('checked'));
        showDeleteButton();  // Verificar si mostrar el botón de eliminar
    });
}

//Función para mostrar el boton eliminar al dar clic individualmente a los chek
function seleccionarCheckboxIndividual() {
    $(document).on('change', '.medicamento-checkbox', function () {
        showDeleteButton();  // Verificar si mostrar el botón de eliminar
    });
}

//Función para mostrar u ocultar el boton eliminar
function showDeleteButton() {
    // Cuenta cuántos checkboxes están seleccionados
    var selectedCount = $('.medicamento-checkbox:checked').length;
    if (selectedCount > 0) {
        $('#deleteButton').removeClass('d-none');
    } else {
        $('#deleteButton').addClass('d-none');
    }
}

//Función para eliminar los usuarios al dar clic al boton.
function clicBtnEliminar() {
    $('#deleteButton').click(function () {
        eliminarUsuariosSeleccionados();  // Eliminar los usuarios seleccionados
    });
}

//Función para llamar el API que elimina medicamentos, además de contemplar eliminación individual de todos los que tienen check
async function eliminarUsuariosSeleccionados() {
    var MedicamentosSeleccionados = [];
    $('.medicamento-checkbox:checked').each(function () {
        MedicamentosSeleccionados.push($(this).closest('tr').data('id'));  // Obtener los IDs de los medicamentos seleccionados
    });

    if (MedicamentosSeleccionados.length > 0) {
        if (confirm('¿Estás seguro de que deseas eliminar los medicamentos seleccionados?')) {
            var deletedCount = 0;
            var errorOccurred = false;

            // Eliminar cada usuario seleccionado de forma secuencial
            for (var i = 0; i < MedicamentosSeleccionados.length; i++) {
                var MedicamentoId = MedicamentosSeleccionados[i];
                var url = "/Medicamentos/Delete/" + MedicamentoId;

                try {
                    const response = await $.ajax({
                        url: url,
                        type: 'POST',
                    });

                    if (response.success) {
                        deletedCount++;
                    } else {
                        errorOccurred = true;
                        alert('Error al eliminar el medicamento ' + MedicamentoId);
                        break;
                    }
                } catch (xhrError) {
                    errorOccurred = true;
                    alert('Error al eliminar el medicamento ' + MedicamentoId + ': ' + xhrError.statusText);
                    break;
                }
            }

            if (!errorOccurred) {
                alert(deletedCount + ' medicamentos han sido eliminados.');
                location.reload();  // Recargar la página para reflejar los cambios
            }
        }
    } else {
        alert('No se seleccionaron medicamentos para eliminar.');  // Alerta si no hay usuarios seleccionados
    }
}