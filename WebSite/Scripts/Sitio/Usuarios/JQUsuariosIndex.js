//Consumo de API GET con paginaci�n.
function cargarUsuarios(page, pageSize, baseUrl) {
    $.ajax({
        url: baseUrl + 'api/Usuarios',  // URL para obtener los usuarios
        type: 'GET',
        data: { page: page, pageSize: pageSize },
        success: function (response) {
            if (response.success) {
                cargarTabla(response);  // Cargar los usuarios en la tabla si la respuesta es exitosa
            } else {
                console.log('Error al obtener los usuarios:', response.data);
            }
        },
        error: function (xhr, status, error) {
            console.log('Error en la solicitud:', error);
        }
    });
}

//Funci�n para crear la tabla usuarios
function cargarTabla(response) {
    $('#usuariosTable tbody').empty();
    $('#pagination').empty();

    response.usuarios.forEach(function (usuario) {
        var row = `<tr data-id="${usuario.idusuario}">
                <td><input type="checkbox" class="user-checkbox" data-id="${usuario.idusuario}" aria-label="Seleccionar usuario"></td>
                <td>${usuario.nombre}</td>
                <td>${usuario.usuario1}</td>
                <td>${new Date(usuario.fechacreacion).toLocaleDateString()}</td>
            </tr>`;
        $('#usuariosTable tbody').append(row);
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
        cargarUsuarios(page, pageSize, baseUrl);  // Obtener los usuarios para la p�gina seleccionada
    });
}

//Funci�n utilizada para que al dar clic sobre cualquier parte de la fila se muestre el modal con los datos de usuario.
function mostrarModalEdicion(urlEditModal) {
    $(document).on('click', 'tr td:not(:first-child)', function () {
        var userId = $(this).closest('tr').data('id'); // Obtener el ID del usuario de la fila
        console.log(userId);
        // Hacer una solicitud para obtener el formulario de edici�n
        
        console.log(urlEditModal);
        $.get(urlEditModal, { id: userId }, function (data) {
            console.log('Se ejecuto2');
            // Inyectar el formulario cargado dentro del modal
            $('#modalContent').html(data);
            // Mostrar el modal
            $('#editModal').modal('show');
        });
    });
}

//Funci�n para desplegar modal al agregar nuevo usuario
function agregarNuevoUsuario(urlEditModal) {
    $(document).on('click', '#addUser', function () {
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
        $('.user-checkbox').prop('checked', $(this).prop('checked'));
        showDeleteButton();  // Verificar si mostrar el bot�n de eliminar
    });
}

//Funci�n para mostrar el boton eliminar al dar clic individualmente a los chek
function seleccionarCheckboxIndividual() {
    $(document).on('change', '.user-checkbox', function () {
        showDeleteButton();  // Verificar si mostrar el bot�n de eliminar
    });
}

//Funci�n para mostrar u ocultar el boton eliminar
function showDeleteButton() {
    // Cuenta cu�ntos checkboxes est�n seleccionados
    var selectedCount = $('.user-checkbox:checked').length;
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

//Funci�n para llamar el API que elimina usuarios, adem�s de contemplar eliminaci�n individual de todos los que tienen check
async function eliminarUsuariosSeleccionados() {
    var selectedUsers = [];
    $('.user-checkbox:checked').each(function () {
        selectedUsers.push($(this).closest('tr').data('id'));  // Obtener los IDs de los usuarios seleccionados
    });

    if (selectedUsers.length > 0) {
        if (confirm('�Est�s seguro de que deseas eliminar los usuarios seleccionados?')) {
            var deletedCount = 0;
            var errorOccurred = false;

            // Eliminar cada usuario seleccionado de forma secuencial
            for (var i = 0; i < selectedUsers.length; i++) {
                var userId = selectedUsers[i];
                var url = "/Usuario/Delete/" + userId;

                try {
                    const response = await $.ajax({
                        url: url,
                        type: 'POST',
                    });

                    if (response.success) {
                        deletedCount++;
                    } else {
                        errorOccurred = true;
                        alert('Error al eliminar el usuario ' + userId);
                        break;
                    }
                } catch (xhrError) {
                    errorOccurred = true;
                    alert('Error al eliminar el usuario ' + userId + ': ' + xhrError.statusText);
                    break;
                }
            }

            if (!errorOccurred) {
                alert(deletedCount + ' usuarios han sido eliminados.');
                location.reload();  // Recargar la p�gina para reflejar los cambios
            }
        }
    } else {
        alert('No se seleccionaron usuarios para eliminar.');  // Alerta si no hay usuarios seleccionados
    }
}
