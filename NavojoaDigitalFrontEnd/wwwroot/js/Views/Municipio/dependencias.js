/*
Tabla de registro de dependencias
*/

class Dependencia {
    constructor() {
        this.id = -1;
        this.clave = '';
        this.nombre = '';
        this.activo = true;
    }

    copyValues(item) {
        this.id = item.id;
        this.clave = item.clave;
        this.nombre = item.nombre;
        this.activo = item.activo;

        return this;
    }

    reset() {
        this.id = -1;
        this.clave = '';
        this.nombre = '';
        this.activo = true;
    }

    updateValues(original, item) {
        original.id = item.id;
        original.clave = item.clave;
        original.nombre = item.nombre;
        original.activo = item.activo;

        return original;
    }
}

var model;
var grid;

$(document).ready(function () {
    model = new Vue({
        el: '#appVue',
        data: {
            dependencias: [],
            current: new Dependencia()
        },
        created() {
            var me = this;
            showSpinner();

            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: '/Municipio/DependenciasGet',
                success: function (result, status, xhr) {

                    hideSpinner();
                    if (result.success == false) {
                        alertify.error(result.error);
                        return;
                    }

                    $.each(result.model, function () {
                        me.dependencias.push(new Dependencia().copyValues(this));
                    })

                    grid = new gridjs.Grid({
                        columns:
                            [
                                {
                                    id: 'id',
                                    hidden: true
                                },
                                {
                                    id: 'clave',
                                    name: 'Clave'
                                },
                                {
                                    id: 'nombre',
                                    name: 'Nombre'
                                },
                                {
                                    id: 'activo',
                                    name: 'Activo',
                                    formatter: (_, row) =>  row.cells[3].data == true ? 'Activo' : 'Inactivo'
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
                        data: me.dependencias,
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
                    url: '/Municipio/DependenciaSave',
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
                            data: me.dependencias
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
                var dependencia = this.dependencias.find(obj => obj.id == result.model.id);
                dependencia.updateValues(dependencia, result.model);
            },
            agregaRegistro: function (result) {
                this.dependencias.push(new Dependencia().copyValues(result.model));
            },
            eliminaRegistro: function (id) {
                var me = this;
                showSpinner();

                $.ajax({
                    type: 'POST',
                    dataType: 'json',
                    data: { id: id },
                    url: '/Municipio/DependenciaDelete',
                    success: function (result, status, xhr) {

                        hideSpinner();

                        if (result.success == false) {
                            alertify.error(result.error);
                            return;
                        }

                        let index = me.dependencias.map((item) => item.id).indexOf(id);
                        if (index >= 0) {
                            me.dependencias.splice(index, 1);

                            grid.updateConfig({
                                data: me.dependencias
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
    if (model.current.clave && model.current.clave.length > 0 &&
        model.current.nombre && model.current.nombre.length > 0)
        return true;

    alertify.warning('Debe indicar todos los campos del registro');
    return false;
}

function editaRegistro(id) {

    if (id < 0) {
        return;
    }

    var dependencia = model.dependencias.find(obj => obj.id == id);
    if (dependencia) {
        model.current = new Dependencia().updateValues(model.current, dependencia);
    }
}

function eliminaRegistro(id) {
    if (id <= 0) {
        return;
    }

    Swal.fire({
        title: "¿Confirma que desea eliminar la dependencia seleccionado?",
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