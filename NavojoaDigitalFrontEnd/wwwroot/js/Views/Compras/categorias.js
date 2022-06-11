/*
Tabla de registro de categorias
*/

class Categoria {
    constructor() {
        this.id = -1;
        this.nombre = '';
    }

    copyValues(item) {
        this.id = item.id;
        this.nombre = item.nombre;

        return this;
    }

    reset() {
        this.id = -1;
        this.nombre = '';
    }

    updateValues(original, item) {
        original.id = item.id;
        original.nombre = item.nombre;

        return original;
    }
}

var model;
var grid;

$(document).ready(function () {
    model = new Vue({
        el: '#appVue',
        data: {
            categorias: [],
            current: new Categoria()
        },
        created() {
            var me = this;
            showSpinner();

            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: '/Compras/CategoriasGet',
                success: function (result, status, xhr) {

                    hideSpinner();

                    if (result.success == false) {
                        alertify.error(result.error);
                        return;
                    }

                    $.each(result.model, function () {
                        me.categorias.push(new Categoria().copyValues(this));
                    })

                    grid = new gridjs.Grid({
                        columns:
                            [
                                {
                                    id: 'id',
                                    hidden: true
                                },
                                {
                                    id: 'nombre',
                                    name: 'Nombre'
                                },
                                {
                                    name: "Acciones",
                                    sort: {
                                        enabled: false
                                    },
                                    formatter: (cell, row) => {
                                        return gridjs.html('<div class="d-flex gap-3"><a href="javascript:void(0);" onclick="editaRegistro(' + row.cells[0].data + ');" data-bs-toggle="modal" data-bs-target=".modalEdit" data-bs-toggle="tooltip" data-bs-placement="top" title="Editar" class="text-success"><i class="mdi mdi-pencil font-size-18"></i></a><a href="javascript:void(0);" onclick="eliminaRegistro(' + row.cells[0].data + ');" data-bs-toggle="tooltip" data-bs-placement="top" title="Delete" class="text-danger"><i class="mdi mdi-delete font-size-18"></i></a></div>');
                                    }
                                }
                            ],
                        pagination: {
                            limit: 8
                        },
                        sort: true,
                        search: true,
                        data: me.categorias,
                        language: {
                            'search': {
                                'placeholder': '🔍 Buscar...'
                            },
                            'pagination': {
                                'previous': 'Anterior',
                                'next': 'Siguiente',
                                'showing': 'Mostrando',
                                'results': () => 'Registros',
                                'to': 'a',
                                'of': 'de'
                            }
                        }
                    }).render(document.getElementById("table-catalog"));

                    $('#btnAdd').show();
                },
                error: function (error) {
                    hideSpinner();
                }
            });
        },
        methods: {
            guardaRegistro: function () {
                var me = this;

                if (!validaRegistro()) {
                    return;
                }

                showSpinner();

                $.ajax({
                    type: 'POST',
                    dataType: 'json',
                    data: model.current,
                    url: '/Compras/CategoriaSave',
                    success: function (result, status, xhr) {

                        hideSpinner();

                        if (result.success == false) {
                            alertify.error(result.error);
                            return;
                        }

                        if (me.current.id > 0) {
                            me.actualizaRegistro(result);
                        } else {
                            me.agregaRegistro(result);
                        }

                        grid.updateConfig({
                            data: me.categorias
                        }).forceRender();

                        $('#modalEdit').modal('hide');

                        alertify.success('Guardado exitoso');
                    },
                    error: function (error) {
                        hideSpinner();
                    }
                });
            },
            actualizaRegistro: function (result) {
                var categoria = this.categorias.find(obj => obj.id == result.model.id);
                categoria.updateValues(categoria, result.model);
            },
            agregaRegistro: function (result) {
                this.categorias.push(new Categoria().copyValues(result.model));
            },
            eliminaRegistro: function (id) {
                var me = this;
                showSpinner();

                $.ajax({
                    type: 'POST',
                    dataType: 'json',
                    data: { id: id },
                    url: '/Compras/CategoriaDelete',
                    success: function (result, status, xhr) {

                        hideSpinner();

                        if (result.success == false) {
                            alertify.error(result.error);
                            return;
                        }

                        let index = me.categorias.map((item) => item.id).indexOf(id);
                        if (index >= 0) {
                            me.categorias.splice(index, 1);

                            grid.updateConfig({
                                data: me.categorias
                            }).forceRender();
                        }
                    },
                    error: function (error) {
                        hideSpinner();
                    }
                });
            }
        }
    });
})

function validaRegistro() {
    if (model.current.nombre && model.current.nombre.length > 0)
        return true;

    alertify.warning('Debe indicar todos los campos del registro');
    return false;
}

function editaRegistro(id) {

    if (id < 0) {
        return;
    }

    var categoria = model.categorias.find(obj => obj.id == id);
    if (categoria) {
        model.current = new Categoria().updateValues(model.current, categoria);
    }
}

function eliminaRegistro(id) {
    if (id <= 0) {
        return;
    }

    Swal.fire({
        title: "¿Confirma que desea eliminar la categoria seleccionado?",
        text: "El proceso no es reversible!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#51d28c",
        cancelButtonColor: "#f34e4e",
        confirmButtonText: "Eliminar",
        cancelButtonText: "Cancelar"
    }).then(function (result) {
        if (result.value) {
            model.eliminaRegistro(id);
        }
    });
}