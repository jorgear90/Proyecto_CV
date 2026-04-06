
// Inicialización segura
$(document).ready(function () {

    // Función principal para actualizar los contadores
    function updateCounter(container) {
        // Encontramos cuántos checkboxes individuales están marcados en esta tabla
        const selectedCount = container.find('.chk-item:checked').length;
        // Actualizamos el número visual en el contador
        container.find('.selected-count').text(selectedCount);

        // Lógica para el checkbox "Seleccionar Todos"
        const totalCheckboxes = container.find('.chk-item').length;
        const selectAllCheckbox = container.find('.select-all');

        // Si hay items y todos están marcados, marcamos el de "Todos"
        if (totalCheckboxes > 0 && selectedCount === totalCheckboxes) {
            selectAllCheckbox.prop('checked', true);
        } else {
            selectAllCheckbox.prop('checked', false);
        }
    }

    // 1. Delegación de eventos para el checkbox "Seleccionar Todos"
    $(document).on('change', '.select-all', function () {
        // Buscamos el contenedor principal de esta opción específica
        const container = $(this).closest('.opcion');
        const isChecked = $(this).prop('checked');

        // Marcamos o desmarcamos todos los checkboxes individuales
        container.find('.chk-item').prop('checked', isChecked);

        // Actualizamos el contador
        updateCounter(container);
    });

    // 2. Delegación de eventos para cada checkbox individual
    $(document).on('change', '.chk-item', function () {
        const container = $(this).closest('.opcion');
        updateCounter(container);
    });

    // 3. Pequeño detalle de UX: Poder hacer clic en toda la fila para seleccionar
    $(document).on('click', '.item-row td:not(:last-child)', function (e) {
        // Evitamos que esto pase si hacen clic directamente en el checkbox
        if ($(e.target).is('input')) return;

        const row = $(this).closest('tr');
        const checkbox = row.find('.chk-item');

        // Invertimos el estado del checkbox
        checkbox.prop('checked', !checkbox.prop('checked'));

        // Actualizamos el contador
        const container = $(this).closest('.opcion');
        updateCounter(container);
    });

    // 4. Inicializar contadores si la vista carga con checkboxes ya marcados
    // (Por ejemplo, al editar un CV existente)
    // Escuchamos un evento personalizado para saber cuándo AJAX terminó de inyectar el HTML
    $(document).on('vistaParcialCargada', function (e, opcionContainer) {
        updateCounter($(opcionContainer));
    });
});

// PUNTO DE ENTRADA PRINCIPAL
function initSelectionTables() {
    // Encontrar todas las tablas con checkboxes
    $('table.table').each(function () {
        initTableSelection($(this));
    });

    // Actualizar contador global
    updateGlobalCounter();
}

// CONFIGURACIÓN DE UNA TABLA ESPECÍFICA
function initTableSelection($table) {
    // Encontrar elementos relativos a esta tabla específica
    const $selectAll = $table.find('.select-all');
    const $checkboxes = $table.find('.chk-item');
    const $selectionCounter = $table.closest('.contenido-parcial').find('.selection-counter') ||
        $table.prev('.selection-counter') ||
        $table.parent().find('.selection-counter');
    const $selectedCount = $selectionCounter.find('.selected-count');
    const $totalCount = $selectionCounter.find('.total-count');

    const totalItems = $checkboxes.length;

    // Establecer contador total
    $totalCount.text(totalItems);

    // Cuando se hace clic en "Seleccionar todos" de ESTA tabla
    $selectAll.off('change').on('change', function () {
        const isChecked = $(this).prop('checked');
        $checkboxes.prop('checked', isChecked);
        updateTableSelectionInfo($table);
        updateGlobalCounter(); // Actualizar contador global
    });

    // Cuando se hace clic en un checkbox individual de ESTA tabla
    $checkboxes.off('change').on('change', function () {
        updateSelectAllCheckbox($table);
        updateTableSelectionInfo($table);
        updateGlobalCounter(); // Actualizar contador global
    });

    // Inicializar estado de esta tabla
    updateSelectAllCheckbox($table);
    updateTableSelectionInfo($table);
}

// LÓGICA DEL CHECKBOX "PADRE"
function updateSelectAllCheckbox($table) {
    const $selectAll = $table.find('.select-all');
    const $checkboxes = $table.find('.chk-item');
    const checkedCount = $checkboxes.filter(':checked').length;
    const totalItems = $checkboxes.length;

    if (checkedCount === 0) {
        // Ninguno seleccionado
        $selectAll.prop('checked', false);
        $selectAll.prop('indeterminate', false);
    } else if (checkedCount === totalItems) {
        // Todos seleccionados
        $selectAll.prop('checked', true);
        $selectAll.prop('indeterminate', false);
    } else {
        // Algunos seleccionados (estado indeterminado)
        $selectAll.prop('checked', false);
        $selectAll.prop('indeterminate', true);
    }
}

// FEEDBACK VISUAL (CONTADORES Y COLORES)
function updateTableSelectionInfo($table) {
    const $selectionCounter = $table.closest('.contenido-parcial').find('.selection-counter') ||
        $table.prev('.selection-counter') ||
        $table.parent().find('.selection-counter');
    const $selectedCount = $selectionCounter.find('.selected-count');
    const $checkboxes = $table.find('.chk-item');

    const checkedCount = $checkboxes.filter(':checked').length;
    $selectedCount.text(checkedCount);

    // Cambiar color del badge según la cantidad seleccionada
    const totalItems = $checkboxes.length;
    $selectionCounter.removeClass('bg-info bg-warning bg-success');

    if (checkedCount === 0) {
        $selectionCounter.addClass('bg-info');
    } else if (checkedCount < totalItems) {
        $selectionCounter.addClass('bg-warning');
    } else {
        $selectionCounter.addClass('bg-success');
    }
}

// CONTADOR GLOBAL
function updateGlobalCounter() {
    // Sumar todos los checkboxes seleccionados en todas las tablas
    const totalSelected = $('.chk-item:checked').length;
    const totalItems = $('.chk-item').length;

    // Actualizar contador global si existe
    const $globalCounter = $('#global-selection-counter');
    if ($globalCounter.length) {
        $globalCounter.find('#global-selected-count').text(totalSelected);
        $globalCounter.find('#global-total-count').text(totalItems);
    }
}

// Función para obtener TODOS los IDs seleccionados de TODAS las tablas
function getAllSelectedIds() {
    const allIds = [];
    $('.chk-item:checked').each(function () {
        allIds.push($(this).val());
    });
    return allIds;
}

// Función para obtener TODOS los items seleccionados de TODAS las tablas
function getAllSelectedItems() {
    const selectedItems = [];
    $('.chk-item:checked').each(function () {
        const $checkbox = $(this);
        const $row = $checkbox.closest('tr'); // Obtenemos la fila padre

        selectedItems.push({
            Id: $row.attr('data-id'),
            Tipo: $row.attr('data-tipo'),

            // Esto es opcional, solo si lo necesitas en el front
            descripcion: $row.find('td:first').text().trim()
        });
    });
    return selectedItems;
}

// Hacer funciones disponibles globalmente
window.getAllSelectedIds = getAllSelectedIds;
window.getAllSelectedItems = getAllSelectedItems;
window.updateGlobalCounter = updateGlobalCounter;