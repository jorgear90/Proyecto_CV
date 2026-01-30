//Este js es para las vistas index de:
    //Conocimientos
    //DatosBasicos
    //ExperienciaLaboral
    //FormacionAcademica
    //Habilidades
    //Links

//Métodos para todos los Index
//Método para eliminar registro de una vistas Index
function confirmarEliminacion(event) {
    event.preventDefault();

    Swal.fire({
        title: '¿Está seguro que desea eliminar este registro?',
        text: 'Esta acción no se puede deshacer.',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Sí, eliminar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            Swal.fire({
                icon: 'success',
                title: 'Registro eliminado',
                text: 'El registro se ha eliminado con éxito.',
                confirmButtonColor: '#005D8F',
                allowOutsideClick: false,
                allowEscapeKey: false
            }).then(() => {
                document.getElementById('submitButtonHidden').value = 'eliminar';
                event.target.closest('form').submit();
            });
        }

    });
}

// Metodos para el Index de CONOCIMIENTOS
//Editar un Conocimiento
$(document).on('submit', '#formEditarConocimiento', function (e) {
    e.preventDefault();

    var form = $(this);

    // Tomar los valores visibles
    var descripcion = form.closest('tr').find('.input-descripcion').val().trim();

    // VALIDACIÓN CLIENTE
    if (descripcion === "") {
        Swal.fire({
            icon: 'warning',
            title: 'Campos requeridos',
            text: 'Debes ingresar una descripción antes de guardar.',
            confirmButtonColor: '#005D8F'
        });
        return; // NO enviar al servidor
    }

    // Pasar valores a los inputs hidden del form
    form.find('.hidden-descripcion').val(descripcion);

    // Token
    var token = $('input[name="__RequestVerificationToken"]').val();

    $.ajax({
        url: '/Conocimientos/Editar',
        method: 'POST',
        data: form.serialize() + "&__RequestVerificationToken=" + token,
        success: function (result) {
            Swal.fire({
                icon: 'success',
                title: 'Conocimiento actualizado',
                confirmButtonColor: '#005D8F'
            });

            const contenedor = $('.opcion[data-url="/Conocimientos/Index"]').find('.contenido-parcial');
            contenedor.html(result);
        },
        error: function (xhr) {
            Swal.fire({
                icon: 'error',
                title: 'Error editar el conocimiento',
                text: xhr.responseText
            });
        }
    });
});

//Método para cancelar
$(document).on('click', '.btn-cancelar-conocimiento', function () {
    const row = $(this).closest('tr');

    const inputDescripcion = row.find('.input-descripcion');

    // Restaurar desde data-original
    inputDescripcion.val(inputDescripcion.data('original'));
});

// Metodos para el Index de DATOSBASICOS
//Datos Básicos
$(document).on('click', '.btn-cancelar-datos-basicos', function () {
    const row = $(this).closest('tr');

    const inputNombre = row.find('.input-nombre');
    const inputValor = row.find('.input-valor');

    // Restaurar desde data-original
    inputNombre.val(inputNombre.data('original'));
    inputValor.val(inputValor.data('original'));
});

//Editar un dato básico
$(document).on('submit', '#formEditarDato', function (e) {
    e.preventDefault();

    var form = $(this);

    // Tomar los valores visibles
    var nombre = form.closest('tr').find('.input-nombre').val().trim();
    var valor = form.closest('tr').find('.input-valor').val().trim();

    // VALIDACIÓN CLIENTE
    if (nombre === "" || valor === "") {
        Swal.fire({
            icon: 'warning',
            title: 'Campos requeridos',
            text: 'Debes completar Datos y Valor antes de guardar.',
            confirmButtonColor: '#005D8F'
        });
        return; // NO enviar al servidor
    }

    // Pasar valores a los inputs hidden del form
    form.find('.hidden-nombre').val(nombre);
    form.find('.hidden-valor').val(valor);

    // Token
    var token = $('input[name="__RequestVerificationToken"]').val();

    $.ajax({
        url: '/DatosBasicos/Editar',
        method: 'POST',
        data: form.serialize() + "&__RequestVerificationToken=" + token,
        success: function (result) {

            const fila = form.closest('tr');

            const inputNombre = fila.find('.input-nombre');
            const inputValor = fila.find('.input-valor');

            inputNombre.data('original', inputNombre.val());
            inputValor.data('original', inputValor.val());

            Swal.fire({
                icon: 'success',
                title: 'Dato actualizado',
                confirmButtonColor: '#005D8F'
            });

            const contenedor = $('.opcion[data-url="/DatosBasicos/Index"]').find('.contenido-parcial');
            contenedor.html(result);
        },
        error: function (xhr) {
            Swal.fire({
                icon: 'error',
                title: 'Error al editar dato básico',
                text: xhr.responseText 
            });
        }
    });
});

// Metodos para el Index de ExperienciaLaboral
//Abrir modal descripción
$(document).on('click', '.descripcion-clickable-el', function () {
    const descripcion = $(this).data('descripcion');
    const id = $(this).data('id');

    const modal = $('#modalDescripcion');
    modal.find('#modal-descripcion').val(descripcion);
    modal.find('#modal-id-editar-ex').val(id);

    modal.find('#modal-descripcion').data('original', descripcion);

    modal.modal('show');
});

//Cancelar edicion modal descripcion
$(document).on('click', '.btn-cancelar-modal-el', function () {
    const textarea = $("#modal-descripcion");

    // Restaurar desde data-original
    const original = textarea.data('original');

    textarea.val(original);
});

//Cancelar edición de un registro
$(document).on('click', '.btn-cancelar-el', function () {
    const row = $(this).closest('tr');

    const inputFechaInicio = row.find('.input-fecha-inicio');
    const inputFechaTermino = row.find('.input-fecha-termino');
    const inputEmpresa = row.find('.input-empresa');

    // Restaurar desde data-original
    inputFechaInicio.val(inputFechaInicio.data('original'));
    inputFechaTermino.val(inputFechaTermino.data('original'));
    inputEmpresa.val(inputEmpresa.data('original'));
});

//Método para editar el modal descripción de Experiencia Laboral
$(document).on('submit', '#formEditarDescripcionEL', function (e) {
    e.preventDefault();

    $.ajax({
        url: '/ExperienciaLaboral/Editar',
        method: 'POST',
        data: $(this).serialize(),
        success: function (result) {

            // Cerrar modal
            $('#modalDescripcion').modal('hide');

            Swal.fire({
                icon: 'success',
                title: 'Descripción actualizada',
                confirmButtonColor: '#005D8F'
            });

            const id = $('#modal-id-editar-ex').val();
            const nuevaDescripcion = $('#modal-descripcion').val();

            $(`.descripcion-clickable-el[data-id="${id}"]`)
                .data('descripcion', nuevaDescripcion); // actualiza el data attribute

            // Recargar parcial si es necesario:
            const contenedor = $('.opcion[data-url="/ExperienciaLaboral/Index"]').find('.contenido-parcial');
            contenedor.html(result);
        },
        error: function (xhr) {
            Swal.fire({
                icon: 'error',
                title: 'Error al guardar descripción',
                text: xhr.responseText
            });
        }
    });
});

//Editar Experiencia laboral
$(document).on('submit', '.edit-form-experiencia-laboral', function (e) { // Usa la clase .edit-form
    e.preventDefault();

    var form = $(this);
    var tr = form.closest('tr'); // Referencia a la fila exacta

    // 1. Tomar los valores visibles
    var fechaInicio = tr.find('.input-fecha-inicio').val().trim();
    var fechaTermino = tr.find('.input-fecha-termino').val().trim();
    var empresa = tr.find('.input-empresa').val().trim();

    // VALIDACIÓN CLIENTE
    if (fechaInicio === "" && empresa === "") {
        Swal.fire({
            icon: 'warning',
            title: 'Campos requeridos',
            text: 'Debes ingresar una fecha de inicio y una empresa.',
            confirmButtonColor: '#005D8F'
        });
        return; // NO enviar al servidor
    } else if (fechaInicio === "") {
        Swal.fire({
            icon: 'warning',
            title: 'Campos requeridos',
            text: 'Debes ingresar una fecha de inicio.',
            confirmButtonColor: '#005D8F'
        });
        return;
    } else if (empresa === "") {
        Swal.fire({
            icon: 'warning',
            title: 'Campos requeridos',
            text: 'Debes ingresar una empresa.',
            confirmButtonColor: '#005D8F'
        });
        return;
    }

    // 2. Pasar valores a los inputs hidden
    form.find('.hidden-fecha-inicio').val(fechaInicio);
    form.find('.hidden-fecha-termino').val(fechaTermino);
    form.find('.hidden-empresa').val(empresa);

    var token = $('input[name="__RequestVerificationToken"]').val();

    $.ajax({
        url: '/ExperienciaLaboral/Editar',
        method: 'POST',
        data: form.serialize() + "&__RequestVerificationToken=" + token,
        success: function (result) {

            const fila = form.closest('tr');

            const inputFechaInicio = fila.find('.input-fecha-inicio');
            const inputFechaTermino = fila.find('.input-fecha-termino');
            const inputEmpresa = fila.find('.input-empresa');

            inputFechaInicio.data('original', inputFechaInicio.val());
            inputFechaTermino.data('original', inputFechaTermino.val());
            inputEmpresa.data('original', inputEmpresa.val());

            Swal.fire({
                icon: 'success',
                title: 'Actualizado',
                text: 'El registro se actualizó correctamente.',
                timer: 1500,
                confirmButtonColor: '#005D8F'
            });

            // --- LÓGICA DE ACTUALIZACIÓN VISUAL ---

            // A. Determinamos si es vigente (si fecha termino está vacía)
            var esVigente = (fechaTermino === "" || fechaTermino === null);

            // B. Buscamos la celda td-vigente en ESTA fila (tr)
            // No hace falta buscar por ID si ya tenemos la fila "tr"
            var tdVigente = tr.find('.td-vigente');

            // C. Actualizamos visualmente
            // ASP.NET Core renderiza los booleanos como <input type="checkbox">
            var checkbox = tdVigente.find('input[type="checkbox"]');

            if (checkbox.length > 0) {
                // Si hay checkbox, lo marcamos o desmarcamos
                checkbox.prop('checked', esVigente);
            } else {
                // Si por alguna razón no hay checkbox, ponemos texto
                tdVigente.text(esVigente ? "Sí" : "No");
            }

            // D. IMPORTANTE: ELIMINAR O COMENTAR ESTA LÍNEA
            // Al hacer esto, sobreescribes la actualización visual que acabamos de hacer
            // con el HTML que devuelve el servidor.

            // const contenedor = ...
            // contenedor.html(result);  <-- ¡NO HAGAS ESTO SI QUIERES AJAX PURO!
        },
        error: function (xhr) {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'No se pudo guardar el cambio.'
            });
        }
    });
});

// Metodos para el Index de FormacionAcademica
//Abrir modal descripción
$(document).on('click', '.descripcion-clickable-fa', function () {
    const descripcion = $(this).data('descripcion');
    const id = $(this).data('id');

    const modal = $('#modalDescripcionFa');
    modal.find('#modal-descripcion-fa').val(descripcion);
    modal.find('#modal-id-editar-fa').val(id);

    modal.find('#modal-descripcion-fa').data('original', descripcion);

    modal.modal('show');
});

//Cancelar edición del modal descripcion
$(document).on('click', '.btn-cancelar-modal-fa', function () {
    const textarea = $("#modal-descripcion-fa");

    // Restaurar desde data-original
    const original = textarea.data('original');

    textarea.val(original);
});

//Cancelar edición de un registro
$(document).on('click', '.btn-cancelar-formacion-academica', function () {
    const row = $(this).closest('tr');

    const inputCarrera = row.find('.input-carrera');
    const inputCiudad = row.find('.input-ciudad');
    const inputNombreInstitucion = row.find('.input-nombre-institucion');
    const inputAnhoInicio = row.find('.input-anho-inicio');
    const inputAnhoTermino = row.find('.input-anho-termino');
    const selectTipo = row.find('.input-tipo-institucion');

    // Restaurar desde data-original
    inputNombreInstitucion.val(inputNombreInstitucion.data('original'));
    inputAnhoInicio.val(inputAnhoInicio.data('original'));
    inputAnhoTermino.val(inputAnhoTermino.data('original'));
    inputCarrera.val(inputCarrera.data('original'));
    inputCiudad.val(inputCiudad.data('original'));
    selectTipo.val(selectTipo.data('original'));
});

//Editar la descripción del modal
$(document).on('submit', '#formEditarDescripcionFa', function (e) {
    e.preventDefault();

    $.ajax({
        url: '/FormacionAcademica/EditarDescripcion',
        method: 'POST',
        data: $(this).serialize(),
        success: function (result) {

            // Cerrar modal
            $('#modalDescripcionFa').modal('hide');

            Swal.fire({
                icon: 'success',
                title: 'Descripción actualizada',
                confirmButtonColor: '#005D8F'
            });

            const id = $('#modal-id-editar-fa').val();
            const nuevaDescripcion = $('#modal-descripcion-fa').val();

            $('.descripcion-clickable-fa[data-id="' + id + '"]')
                .data('descripcion', nuevaDescripcion); // actualiza el data attribute

            // Recargar parcial si es necesario:
            const contenedor = $('.opcion[data-url="/FormacionAcademica/Index"]').find('.contenido-parcial');
            contenedor.html(result);
        },
        error: function (xhr) {
            Swal.fire({
                icon: 'error',
                title: 'Error al guardar descripción',
                text: xhr.responseText
            });
        }
    });
});

//Editar un registro
$(document).on('submit', '.edit-form-formacion-academica', function (e) { // Usa la clase .edit-form
    e.preventDefault();

    var form = $(this);
    var tr = form.closest('tr'); // Referencia a la fila exacta

    // 1. Tomar los valores visibles
    var nombreInstitucion = form.closest('tr').find('.input-nombre-institucion').val().trim();
    var carrera = form.closest('tr').find('.input-carrera').val().trim();
    var ciudad = form.closest('tr').find('.input-ciudad').val().trim();
    var anhoInicio = form.closest('tr').find('.input-anho-inicio').val().trim();
    var anhoTermino = form.closest('tr').find('.input-anho-termino').val().trim();
    var tipoInstitucion = form.closest('tr').find('.input-tipo-institucion').val().trim();

    // VALIDACIÓN CLIENTE
    if (carrera === "" && anhoInicio === "" && nombreInstitucion === "" && ciudad === "") {
        Swal.fire({
            icon: 'warning',
            title: 'Campos requeridos',
            text: 'Debes ingresar un nombre una carrera y un año de inicio.',
            confirmButtonColor: '#005D8F'
        });
        return; // NO enviar al servidor
    } else if (anhoInicio === "") {
        Swal.fire({
            icon: 'warning',
            title: 'Campos requeridos',
            text: 'Debes ingresar un año de inicio.',
            confirmButtonColor: '#005D8F'
        });
        return;
    } else if (carrera === "") {
        Swal.fire({
            icon: 'warning',
            title: 'Campos requeridos',
            text: 'Debes ingresar una carrera.',
            confirmButtonColor: '#005D8F'
        });
        return;
    } else if (nombreInstitucion === "") {
        Swal.fire({
            icon: 'warning',
            title: 'Campos requeridos',
            text: 'Debes ingresar un nombre.',
            confirmButtonColor: '#005D8F'
        });
        return;
    } else if (ciudad === "") {
        Swal.fire({
            icon: 'warning',
            title: 'Campos requeridos',
            text: 'Debes ingresar una ciudad.',
            confirmButtonColor: '#005D8F'
        });
        return;
    }

    // 2. Pasar valores a los inputs hidden
    form.find('.hidden-anho-inicio').val(anhoInicio);
    form.find('.hidden-anho-termino').val(anhoTermino);
    form.find('.hidden-carrera').val(carrera);
    form.find('.hidden-ciudad').val(ciudad);
    form.find('.hidden-nombre-institucion').val(nombreInstitucion);
    form.find('.hidden-tipo-institucion').val(tipoInstitucion);

    var token = $('input[name="__RequestVerificationToken"]').val();

    $.ajax({
        url: '/FormacionAcademica/Editar',
        method: 'POST',
        data: form.serialize() + "&__RequestVerificationToken=" + token,
        success: function (result) {

            const fila = form.closest('tr');

            const inputNombreInstitucion = fila.find('.input-nombre-institucion');
            const inputCiudad = fila.find('.input-ciudad');
            const inputCarrera = fila.find('.input-carrera');
            const inputAnhoInicio = fila.find('.input-anho-inicio');
            const inputAnhoTermino = fila.find('.input-anho-termino');
            const inputTipoInstitucion = fila.find('.input-tipo-institucion');

            inputNombreInstitucion.data('original', inputNombreInstitucion.val());
            inputCiudad.data('original', inputCiudad.val());
            inputCarrera.data('original', inputCarrera.val());
            inputAnhoInicio.data('original', inputAnhoInicio.val());
            inputAnhoTermino.data('original', inputAnhoTermino.val());
            inputTipoInstitucion.data('original', inputTipoInstitucion.val());

            Swal.fire({
                icon: 'success',
                title: 'Actualizado',
                text: 'El registro se actualizó correctamente.',
                timer: 1500,
                confirmButtonColor: '#005D8F'
            });

            // --- LÓGICA DE ACTUALIZACIÓN VISUAL ---

            // A. Determinamos si es vigente (si fecha termino está vacía)
            var esVigente = (anhoTermino === "" || anhoTermino === null);

            // B. Buscamos la celda td-vigente en ESTA fila (tr)
            // No hace falta buscar por ID si ya tenemos la fila "tr"
            var tdVigente = tr.find('.td-vigente');

            // C. Actualizamos visualmente
            // ASP.NET Core renderiza los booleanos como <input type="checkbox">
            var checkbox = tdVigente.find('input[type="checkbox"]');

            if (checkbox.length > 0) {
                // Si hay checkbox, lo marcamos o desmarcamos
                checkbox.prop('checked', esVigente);
            } else {
                // Si por alguna razón no hay checkbox, ponemos texto
                tdVigente.text(esVigente ? "Sí" : "No");
            }

            // D. IMPORTANTE: ELIMINAR O COMENTAR ESTA LÍNEA
            // Al hacer esto, sobreescribes la actualización visual que acabamos de hacer
            // con el HTML que devuelve el servidor.

            // const contenedor = ...
            // contenedor.html(result);  <-- ¡NO HAGAS ESTO SI QUIERES AJAX PURO!
        },
        error: function (xhr) {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'No se pudo guardar el cambio.'
            });
        }
    });
});

// Metodos para el Index de HABILIDADES
//Restaurar valores predeterminados o cancelar edición
$(document).on('click', '.btn-cancelar-habilidad', function () {
    const row = $(this).closest('tr');

    const inputDescripcion = row.find('.input-descripcion');

    // Restaurar desde data-original
    inputDescripcion.val(inputDescripcion.data('original'));
});

//Editar una habilidad
$(document).on('submit', '#formEditarHabilidad', function (e) {
    e.preventDefault();

    var form = $(this);

    // Tomar los valores visibles
    var descripcion = form.closest('tr').find('.input-descripcion').val().trim();

    // VALIDACIÓN CLIENTE
    if (descripcion === "") {
        Swal.fire({
            icon: 'warning',
            title: 'Campos requeridos',
            text: 'Debes ingresar una descripción antes de guardar.',
            confirmButtonColor: '#005D8F'
        });
        return; // NO enviar al servidor
    }

    // Pasar valores a los inputs hidden del form
    form.find('.hidden-descripcion').val(descripcion);

    // Token
    var token = $('input[name="__RequestVerificationToken"]').val();

    $.ajax({
        url: '/Habilidades/Editar',
        method: 'POST',
        data: form.serialize() + "&__RequestVerificationToken=" + token,
        success: function (result) {

            const fila = form.closest('tr');

            const inputDescripcion = fila.find('.input-descripcion');

            inputDescripcion.data('original', inputDescripcion.val());

            Swal.fire({
                icon: 'success',
                title: 'Habilidad actualizada',
                confirmButtonColor: '#005D8F'
            });

            const contenedor = $('.opcion[data-url="/Habilidades/Index"]').find('.contenido-parcial');
            contenedor.html(result);
        },
        error: function (xhr) {
            Swal.fire({
                icon: 'error',
                title: 'Error editar la habilidad',
                text: xhr.responseText
            });
        }
    });
});

// Metodos para el Index de LINKS
//Links
$(document).on('click', '.btn-cancelar-enlace', function () {
    const row = $(this).closest('tr');

    const inputTitulo = row.find('.input-titulo');
    const inputEnlace = row.find('.input-enlace');

    // Restaurar desde data-original
    inputTitulo.val(inputTitulo.data('original'));
    inputEnlace.val(inputEnlace.data('original'));
});

//Editar enlace
$(document).on('submit', 'form[data-form="editar-enlace"]', function (e) {
    e.preventDefault();

    var form = $(this);

    // Tomar los valores visibles
    var titulo = form.closest('tr').find('.input-titulo').val().trim();
    var enlace = form.closest('tr').find('.input-enlace').val().trim();

    // VALIDACIÓN CLIENTE
    if (titulo === "" || enlace === "") {
        Swal.fire({
            icon: 'warning',
            title: 'Campos requeridos',
            text: 'Debes completar Titulo y Enlace antes de guardar.',
            confirmButtonColor: '#005D8F'
        });
        return; // NO enviar al servidor
    }

    // Pasar valores a los inputs hidden del form
    form.find('input[name="Titulo"]').val(titulo);
    form.find('input[name="Enlace"]').val(enlace);

    // Token
    var token = $('input[name="__RequestVerificationToken"]').val();

    $.ajax({
        url: form.attr('action'),
        method: 'POST',
        data: form.serialize(),
        success: function (result) {

            const fila = form.closest('tr');

            const inputTitulo = fila.find('.input-titulo');
            const inputEnlace = fila.find('.input-enlace');

            inputTitulo.data('original', inputTitulo.val());
            inputEnlace.data('original', inputEnlace.val());

            Swal.fire({
                icon: 'success',
                title: 'Registro actualizado',
                confirmButtonColor: '#005D8F'
            });

            const contenedor = $('.opcion[data-url="/Links/Index"]').find('.contenido-parcial');
            contenedor.html(result);
        },
        error: function (xhr) {
            Swal.fire({
                icon: 'error',
                title: 'Error al editar registro',
                text: xhr.responseText
            });
        }
    });
});
