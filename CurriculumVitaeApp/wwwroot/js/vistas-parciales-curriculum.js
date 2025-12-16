$(document).ready(function () {
    // Inicializar todas las tablas en la página
    initSelectionTables();

    // También inicializar cuando se cargue contenido dinámico (si usas AJAX)
    $(document).on('contentLoaded', function () {
        initSelectionTables();
    });
});

function initSelectionTables() {
    // Encontrar todas las tablas con checkboxes
    $('table.table').each(function () {
        initTableSelection($(this));
    });

    // Actualizar contador global
    updateGlobalCounter();
}

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
        const $row = $checkbox.closest('tr');
        selectedItems.push({
            id: $checkbox.val(),
            tipo: $row.data('tipo'),
            descripcion: $row.find('td:first').text().trim()
        });
    });
    return selectedItems;
}

// Hacer funciones disponibles globalmente
window.getAllSelectedIds = getAllSelectedIds;
window.getAllSelectedItems = getAllSelectedItems;
window.updateGlobalCounter = updateGlobalCounter;