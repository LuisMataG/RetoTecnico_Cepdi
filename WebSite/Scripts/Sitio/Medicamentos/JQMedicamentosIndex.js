//Consumo de API GET con paginaci�n.
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
//Funci�n para crear la tabla medicamentos
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

    // Crear botones de paginaci�n despu�s de cargar la tabla
    crearPaginacion(response.paginaActual, response.totalPaginas);
}
// Funci�n para crear los botones de paginaci�n
function crearPaginacion(paginaActual, totalPaginas) {
    var paginationHTML = '';

    // Crear un bot�n para la p�gina anterior
    if (paginaActual > 1) {
        paginationHTML += `<button class="pagination-btn" data-page="${paginaActual - 1}"><</button>`;
    }

    // Crear botones para cada p�gina
    for (var i = 1; i <= totalPaginas; i++) {
        if (i === paginaActual) {
            paginationHTML += `<button class="pagination-btn active" data-page="${i}">${i}</button>`;
        } else {
            paginationHTML += `<button class="pagination-btn" data-page="${i}">${i}</button>`;
        }
    }

    // Crear un bot�n para la p�gina siguiente
    if (paginaActual < totalPaginas) {
        paginationHTML += `<button class="pagination-btn" data-page="${paginaActual + 1}">></button>`;
    }

    // Insertar los botones de paginaci�n en el contenedor
    $('#pagination').append(paginationHTML);
}

// Funci�n para manejar el clic en los botones de paginaci�n
function cambiarDePagina(pageSize, baseUrl) {
    $(document).on('click', '.pagination-btn', function () {
        var page = $(this).data('page');  // Obtener el n�mero de p�gina seleccionado
        console.log('Page: ' + page + '. Tama�o: ' + pageSize);
        cargarMedicamentos(page, pageSize, baseUrl);  // Obtener los medicamentos para la p�gina seleccionada
    });
}

//Funci�n utilizada para que al dar clic sobre cualquier parte de la fila se muestre el modal con los datos del medicamento.
function mostrarModalEdicion(urlModal) {
    $(document).on('click', 'tr td:not(:first-child)', function () {
        var medicamentoId = $(this).closest('tr').data('id'); // Obtener el ID del medicamento de la fila
        console.log(medicamentoId);
        // Hacer una solicitud para obtener el formulario de edici�n

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

//Funci�n para desplegar modal al agregar nuevo medicamento
function agregarNuevoMedicamento(urlEditModal) {
    $(document).on('click', '#addMedic', function () {
        $.get(urlEditModal, { id: 0 }, function (data) {
            $('#modalContent').html(data);
            $('#editModal').modal('show');
        });
    });
}

//Funci�n para el seleccionar todos los checkbox y mostrar el boton eliminar.
function seleccionarTodosChkBox() {
    $('#selectAll').change(function () {
        // Cambiar el estado de todos los checkboxes de la tabla
        $('.medicamento-checkbox').prop('checked', $(this).prop('checked'));
        showDeleteButton();  // Verificar si mostrar el bot�n de eliminar
    });
}

//Funci�n para mostrar el boton eliminar al dar clic individualmente a los chek
function seleccionarCheckboxIndividual() {
    $(document).on('change', '.medicamento-checkbox', function () {
        showDeleteButton();  // Verificar si mostrar el bot�n de eliminar
    });
}

//Funci�n para mostrar u ocultar el boton eliminar
function showDeleteButton() {
    // Cuenta cu�ntos checkboxes est�n seleccionados
    var selectedCount = $('.medicamento-checkbox:checked').length;
    if (selectedCount > 0) {
        $('#deleteButton').removeClass('d-none');
    } else {
        $('#deleteButton').addClass('d-none');
    }
}

//Funci�n para eliminar los usuarios al dar clic al boton.
function clicBtnEliminar() {
    $('#deleteButton').click(function () {
        eliminarUsuariosSeleccionados();  // Eliminar los usuarios seleccionados
    });
}

//Funci�n para llamar el API que elimina medicamentos, adem�s de contemplar eliminaci�n individual de todos los que tienen check
async function eliminarUsuariosSeleccionados() {
    var MedicamentosSeleccionados = [];
    $('.medicamento-checkbox:checked').each(function () {
        MedicamentosSeleccionados.push($(this).closest('tr').data('id'));  // Obtener los IDs de los medicamentos seleccionados
    });

    if (MedicamentosSeleccionados.length > 0) {
        if (confirm('�Est�s seguro de que deseas eliminar los medicamentos seleccionados?')) {
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
                location.reload();  // Recargar la p�gina para reflejar los cambios
            }
        }
    } else {
        alert('No se seleccionaron medicamentos para eliminar.');  // Alerta si no hay usuarios seleccionados
    }
}