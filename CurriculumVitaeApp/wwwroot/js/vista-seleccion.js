function cargarVistaParcial(container, url) {

    // 🔥 Obtener el idCv del hidden
    let idCv = $('#idCvHidden').val();

    // 🔥 Agregarlo a la URL
    let urlFinal = url + '?idCv=' + idCv;

    $.ajax({
        url: urlFinal,
        type: 'GET',
        success: function (result) {
            container.hide().html(result);

            activarOrdenamiento(container);

            container.slideDown();
        },
        error: function () {
            container.html('<p class="text-danger">Error al cargar la vista.</p>');
        }
    });
}


// 1. Variable global para recordar qué categorías ya trajimos por AJAX
let categoriasCargadas = [];

$(document).ready(function () {

    function abrirOpcion(opcionElement) {
        const parent = opcionElement;
        const contentContainer = parent.find('.contenido-parcial');
        const url = parent.data('url');
        const toggleButton = parent.find('.btn-toggle');
        const tipo = parent.data('tipo').toString(); // Obtenemos el tipo

        $('.contenido-parcial').not(contentContainer).slideUp();
        $('.btn-toggle').not(toggleButton).text('+');

        // Registramos que esta categoría ya fue abierta y sus datos están en el DOM
        if (!categoriasCargadas.includes(tipo)) {
            categoriasCargadas.push(tipo);
        }

        cargarVistaParcial(contentContainer, url);
        toggleButton.text('–');
    }

    $('.btn-toggle').click(function () {
        const parent = $(this).closest('.opcion');
        const contentContainer = parent.find('.contenido-parcial');
        const url = parent.data('url');
        const tipo = parent.data('tipo').toString(); // Obtenemos el tipo
        const toggleButton = $(this);

        $('.contenido-parcial').not(contentContainer).slideUp();
        $('.btn-toggle').not(toggleButton).text('+');

        if (contentContainer.is(':visible')) {
            contentContainer.slideUp();
            toggleButton.text('+');
        } else {
            if (contentContainer.children().length > 0) {
                contentContainer.slideDown();
                toggleButton.text('–');
            } else {
                // Registramos la categoría
                if (!categoriasCargadas.includes(tipo)) {
                    categoriasCargadas.push(tipo);
                }
                cargarVistaParcial(contentContainer, url);
                toggleButton.text('–');
            }
        }
    });

    abrirOpcion($('.opcion[data-url="/Curriculum/SelectorDatosBasicos"]'));
});

function activarOrdenamiento(contenedor) {
    const tbody = contenedor.find("tbody")[0]; // obtener DOM nativo

    if (!tbody) return; // no hacer nada si no existe (evita errores)

    new Sortable(tbody, {
        animation: 150,
        handle: ".item-row",
        ghostClass: "sortable-ghost"
    });
}

//Confirmar la selección Credential los items y generar PDF
function confirmarSeleccion(event) {
    event.preventDefault();

    var form = $('#formSeleccion');
    var valor = $('.input-valor').val()?.trim();
    var nombreCv = $('.input-nombre-cv').val()?.trim();
    var profesion = $('.input-profesion').val()?.trim();
    var idCv = $('.input-id-cv').val()?.trim();

    if (!valor) {
        Swal.fire({
            icon: 'warning',
            title: 'Encabezado requerido',
            text: 'Debes ingresar un encabezado antes de generar el PDF',
            confirmButtonColor: '#005D8F'
        });
        return;
    } else if (!nombreCv) {
        Swal.fire({
            icon: 'warning',
            title: 'Nombre requerido',
            text: 'Debes ingresar un nombre para el cv antes de generar el PDF',
            confirmButtonColor: '#005D8F'
        });
        return;
    }

    form.find('.hidden-encabezado').val(valor);
    form.find('.hidden-nombre-cv').val(nombreCv);
    form.find('.hidden-profesion').val(profesion);
    form.find('.hidden-id-cv').val(idCv);

    Swal.fire({
        title: 'Generar curriculum',
        text: '¿Deseas generar el PDF con los elementos seleccionados?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#005D8F',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Sí, generar PDF',
        cancelButtonText: 'Revisar selección'
    }).then((result) => {
        if (result.isConfirmed) {

            let seleccionados = [];

            // PASO A: Leer lo que venía de la base de datos inicialmente
            let seleccionesPrevias = JSON.parse($("#seleccionesPreviasHidden").val() || "[]");

            // PASO B: Si NO abrimos la pestaña, conservamos los datos previos
            seleccionesPrevias.forEach(function (item) {
                // Si la categoría del item NO está en las cargadas, lo conservamos intacto
                if (!categoriasCargadas.includes(item.Tipo)) {
                    seleccionados.push({
                        Id: item.Id,
                        Tipo: item.Tipo
                    });
                }
            });

            // PASO C: Iteramos sobre el DOM para sacar lo de las pestañas que SÍ se abrieron
            $(".item-row").each(function () {
                const chk = $(this).find(".chk-item");

                if (chk.is(":checked")) {
                    var idEncriptado = $(this).attr("data-id");
                    var tipoDato = $(this).attr("data-tipo");

                    seleccionados.push({
                        Id: idEncriptado,
                        Tipo: tipoDato
                    });
                }
            });

            $("#seleccionadosJson").val(JSON.stringify(seleccionados));

            Swal.fire({
                icon: 'success',
                title: 'Generando PDF...',
                text: 'Tu curriculum está siendo preparado.',
                confirmButtonColor: '#005D8F',
                allowOutsideClick: false,
                allowEscapeKey: false,
                timer: 1400,
                showConfirmButton: true,
            });


            setTimeout(() => {
                form.submit();
            }, 1400);
        }
    });
}

//Entrega un error si se alcanza el maximo de cvs almacenados
$(document).ready(function () {
    if (window.swalErrorMessage) {
        Swal.fire({
            icon: 'error',
            title: 'No es posible continuar',
            text: window.swalErrorMessage,
            confirmButtonColor: '#d33'
        });
    }
});

//Modales
$(document).on('hidden.bs.modal', function () {
    $('.modal-backdrop').remove();
    $('body').removeClass('modal-open');
});