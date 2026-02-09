function cargarVistaParcial(container, url) {
    $.ajax({
        url: url,
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

$(document).ready(function () {
    // Función para abrir una opción específica
    function abrirOpcion(opcionElement) {
        const parent = opcionElement;
        const contentContainer = parent.find('.contenido-parcial');
        const url = parent.data('url');
        const toggleButton = parent.find('.btn-toggle');

        // Cierra las otras vistas parciales
        $('.contenido-parcial').not(contentContainer).slideUp();
        $('.btn-toggle').not(toggleButton).text('+');

        // Carga y muestra la vista parcial
        cargarVistaParcial(contentContainer, url);
        toggleButton.text('–');
    }

    $('.btn-toggle').click(function () {
        const parent = $(this).closest('.opcion');
        const contentContainer = parent.find('.contenido-parcial');
        const url = parent.data('url');
        const toggleButton = $(this);

        // Cierra las otras vistas parciales
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
                cargarVistaParcial(contentContainer, url);
                toggleButton.text('–');
            }
        }
    });

    // Abrir automáticamente "Datos Básicos" al cargar
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

            // Iteramos sobre las filas
            $(".item-row").each(function () {
                const chk = $(this).find(".chk-item");

                if (chk.is(":checked")) {

                    // CAMBIO CRÍTICO 1: Usar .attr() en vez de .data()
                    // .attr() obliga a leer el TEXTO exacto del HTML (el string encriptado)
                    var idEncriptado = $(this).attr("data-id");
                    var tipoDato = $(this).attr("data-tipo");

                    seleccionados.push({
                        // CAMBIO CRÍTICO 2: Poner la primera letra en MAYÚSCULA (Id, Tipo)
                        // para que coincida exactamente con tu clase C# ItemSeleccion
                        Id: idEncriptado,
                        Tipo: tipoDato
                    });
                }
            });

            // DEBUG: Muestra esto en la consola (F12) antes de que se envíe
            //console.log("JSON a enviar:", JSON.stringify(seleccionados));

            // Asignamos el valor al input hidden
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
                // Enviar el formulario
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