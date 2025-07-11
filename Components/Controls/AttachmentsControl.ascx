<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AttachmentsControl.ascx.cs" Inherits="AllNet.Modules.ReservasExportaciones.Components.Controls.AttachmentsControl" %>
<div class="modal fade " id="myModal" tabindex="-1" role="dialog" data-backdrop="static" aria-labelledby="myModalLabel" >
    <div class="modal-dialog modal-lg modal-vertical-centered modal-dialog-custom modal-lg-custom" role="document">
        <div class="modal-content modal-content-custom">
            <div class="modal-header">
                <h4 class="modal-title text-center" id="myModalLabel">Archivos adjuntos</h4>
                <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body tabla-contenedor" id="modal-body">
                <div class="tabla-contenedor-tbl">
                    <table cellpadding="0" cellspacing="0" border="0" class="table table-striped table-bordered display align-middle text-center" id="tbl_files" width="100%">
                        <thead>
                        </thead>
                    </table>
                </div>
            </div>             
        </div>
    </div>
</div>
 <div class="modal fade contenedorModal" id="solicitarModal" tabindex="-1" role="dialog" aria-labelledby="modalLabel" aria-hidden="true">
     <div class="modal-dialog" role="document">
         <div class="modal-content">
             <div class="modal-header">
                 <h5 class="modal-title" id="modalLabel"><i class="bi bi-folder-check"></i>Actualizar estado</h5>
                 <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                     <span aria-hidden="true">&times;</span>
                 </button>
             </div>
             <div class="modal-body" id="modalBodyContent">
                 <!-- Contenido del modal dinámico -->
             </div>
             <div class="modal-footer">
                 <button type="button" class="btn btn-primary" id="enviarEstado">Cambiar estado</button>
                 <button type="button" class="btn btn-secondary" data-dismiss="modal">Cerrar</button>
             </div>
         </div>
     </div>
 </div>
<script>
    function mostrarModal(bl, codigo, control, isagent) {
        debugger;
        var selectedOptionId = control.options[control.selectedIndex].id;

        if (isagent == "True") {
                $('#modalBodyContent').html(`
           <p class="text-danger text-center">
             <i class="bi bi-question-circle"></i>¿Desea actualizar el estado a: ${obtenerEstado(selectedOptionId)}?
           </p>
           `);
           
            if ($('#enviarEstado').hasClass('hidden')) {
                $('#enviarEstado').removeClass('hidden');
            }
           
            $('#enviarEstado')
                .data('bl', bl)
                .data('estado', selectedOptionId)
                .data('codigo', codigo);
           
            $('#solicitarModal').modal('show');
        }
    }

    function mostrarModal2() {
    
        debugger;
        /*var selectedOptionId = control.options[control.selectedIndex].id;*/
        $('#modalBodyContent').html(`
        <p class="text-danger text-center">
          <i class="bi bi-question-circle"></i>Estimado cliente, favor tener en cuenta que el cut-off físico para recibo de SAES se encuentra vencido, el envío tardío tiene un costo aplicable de $190.000 + IVA
        </p>
        `);       
        $('#enviarEstado').addClass('hidden');
        $('#solicitarModal').modal('show');
    }

    function obtenerEstado(id) {
        switch (id) {
            case '0':
                return "Pendiente"
            case '1':
                return "Completo"
            case '2':
                return "Incompleto"            
        }
    }

    $(document).on('click', '#enviarEstado', function () {
        var bl = $(this).data('bl');
        var estado = $(this).data('estado');
        var codigo = $(this).data('codigo');
        $.ajax({
            url: urlApi + 'FilesExpo/UpdateState',
            beforeSend: sf.setModuleHeaders,
            destroy:true,
            data: {
                cn: cn_hash,
                pkey: pkey,
                nituser: nitUser,
                state_id: estado,
                isagent: isAgent,
                Booking: bl,
                Codigo: codigo
            },
            type: 'PUT',
            success: function (response) {
                $('#modalBodyContent').html(`
                <p class="text-success text-center">
                  <i class="bi bi-check-circle"></i> ¡Estado actualizado con éxito!
                </p>
                `);
                $('#enviarEstado').addClass('hidden')
            },
            error: function (error) {
                console.error(error);
            }
        });
    });

    function mensajeCutOff(cutoff) {
        if (typeof cutoff === 'string' && cutoff.includes('/')) {
            const [datePart, timePart] = cutoff.split(' ');
            const [day, month, year] = datePart.split('/');
            cutoff = `${year}-${month}-${day}T${timePart || '00:00:00'}`;
        }

        if (/T\d:/.test(cutoff) && !/T\d{2}:/.test(cutoff)) {
            cutoff = cutoff.replace(/T(\d):/, "T0$1:");
        }

        const cutoffDate = new Date(cutoff);
        if (isNaN(cutoffDate)) {
            console.error("Fecha cutoff inválida:", cutoff);
            return;
        }

        const now = new Date();

        // Mostrar el modal solo si ya se alcanzó o pasó el cutoff
        if (now >= cutoffDate) {
            mostrarModal2();
        }
    }


    function leerDocumentos(bl, cutoff) {
        mensajeCutOff(cutoff);
        tableFiles = $('#tbl_files').DataTable({
            dom: 'Bfrtip',
            serverSide: true,
            processing: true,
            destroy: true,
            order: [1, 'asc'],
            ajax: {
                url: urlApi + 'FilesExpo/Get',
                beforeSend: sf.setModuleHeaders,
                data: function (data) {
                    data.cn = cn_hash;
                    data.pkey = pkey;
                    data.Booking = bl;
                    data.nituser = nitUser;
                    data.isagent = isAgent;
                    return data;
                },
                type: 'POST'
            },
            columns: [
                {
                    className: 'dt-control',
                    orderable: false,
                    searchable: false,
                    data: null,
                    defaultContent: '',
                    width: '10%'
                },
                {
                    title: "RESERVA",
                    data: "BOOKINGS_EXPO_DOCS.BOOKING",
                    key: true
                },
                {
                    title: "CODIGO",
                    data: "BOOKINGS_EXPO_DOCS.CODIGO"
                },
                {
                    title: "CATEGORIA",
                    data: "BOOKINGS_EXPO_DOCS.CATEGORIA"
                },
                {
                    title: "PESO",
                    data: "BOOKINGS_EXPO_DOCS.PESO"
                },
                {
                    title: "BULTOS",
                    data: "BOOKINGS_EXPO_DOCS.BULTOS"
                },
                {
                    title: "COMENTARIOS",
                    data: "BOOKINGS_EXPO_DOCS.COMENTARIO",                   
                    className: "dt-head-center"
                },             
                {
                    title: "ADJUNTO",
                    data: "BOOKINGS_EXPO_DOCS.ADJUNTO",
                    render: function (file_id) {
                        return file_id ?
                            `<a href="${editor.file('ARCHIVOS_BOOKINGS', file_id).web_path}" target="_blank">Ver</a>` :
                            null;
                    },
                    defaultContent: "",
                },
                {
                    title: "Estado",
                    data: "ESTADO",
                    render: function (data, type, row) {                

                        let selectOptions = [
                            { value: '0', id: '0',text:'Pendiente' },
                            { value: '1', id: '1',text:'Completo' },
                            { value: '2', id: '2',text:'Incompleto' }
                         ];
           
                        let selectHtml = `<select id="sl_state" 
                           onclick="event.stopPropagation()" 
                           onchange="mostrarModal('${row.BOOKINGS_EXPO_DOCS.BOOKING}','${row.BOOKINGS_EXPO_DOCS.CODIGO}',this,'${isAgent}')" 
                           ${isAgent=="True" ? "" : "disabled"}>`;

                        for (let option of selectOptions) {
                            selectHtml += `<option id="${option.id}" value="${option.value}" ${data === option.value ? 'selected' : ''}>${option.text}</option>`;
                        }

                        selectHtml += `</select>`;

                        return selectHtml;
                    },
                    className: "dt-head-center"
                }            

            ],
            select: {
                style: 'os',
                selector: 'td:not(:first-child)'
            },

            lengthChange: false,
            language: lang,
            columnDefs: [
                {
                    targets: [6],
                    createdCell: function (td, cellData, rowData, row, col) {
                        $(td).css({
                            'white-space': 'normal',
                            'word-wrap': 'break-word',
                            'max-width': '300px',
                            'text-align': 'left',
                            'padding-left': '10px'
                        });
                    }
                }
            ],
            buttons: [
                {
                    extend: 'create', editor: editor,
                    className: 'btn btn-primary',
                    formButtons: [
                        {
                            label: 'Crear registro',
                            className: 'btn btn-success btn-crear',
                            fn: function () {
                                this.submit();
                            }
                        },
                        {
                            label: 'Cerrar ventana',
                            className: 'btn btn-danger',
                            fn: function () {
                                this.close();
                            }
                        }
                    ]

                },
                { extend: 'edit', className: 'btn btn-primary', editor: editor },
                { extend: 'remove', className: 'btn btn-primary', editor: editor }

            ]
        });      



        
        tableFiles.off('click', 'td.dt-control').on('click', 'td.dt-control', function () {

            let tr = $(this).closest('tr');

            btn_child = $(this);

            let row = tableFiles.row(tr);

            if (row.child.isShown()) {

                destroyChild(row);
                tr.removeClass('shown');

            }
            else {
                // Open this row
                createChild(row);
                tr.addClass('shown');
            }
        });


        function createChild(row) {
            tableChild = $('<table cellpadding="0" cellspacing="0" border="0" class="display" width="100%"/>');


            editorCntr = new DataTable.Editor({
                ajax: {
                    create: {
                        type: 'POST',
                        url: urlApi + 'Cntr/Create',
                        beforeSend: sf.setModuleHeaders,
                        data: function (data) {
                            data.cn = cn_hash;
                            data.pkey = pkey;
                            data.nituser = nitUser;
                            data.isagent = isAgent;
                            data.Booking = booking;
                            data.Codigo = tableFiles.row({ selected: true }).data() !== undefined ? tableFiles.row({ selected: true }).data().CODIGO : '';
                            data.ID = row.data().ID;
                            return data;
                        }
                    },
                    edit: {
                        type: 'PUT',
                        url: urlApi + 'Cntr/Edit',
                        beforeSend: sf.setModuleHeaders,
                        data: function (data) {
                            data.cn = cn_hash;
                            data.pkey = pkey;
                            data.nituser = nitUser;
                            data.isagent = isAgent;
                            data.Booking = booking;
                            data.Codigo = tableFiles.row({ selected: true }).data() !== undefined ? tableFiles.row({ selected: true }).data().CODIGO : '';
                            data.ID = row.data().ID;
                            return data;
                        }
                    },
                    remove: {
                        type: 'POST',
                        url: urlApi + 'Cntr/Remove',
                        beforeSend: sf.setModuleHeaders,
                        data: function (data) {
                            data.cn = cn_hash;
                            data.pkey = pkey;
                            data.nituser = nitUser;
                            data.isagent = isAgent;
                            data.Booking = booking;
                            /*  data.Codigo = tableFiles.row({ selected: true }).data() !== undefined ? tableFiles.row({ selected: true }).data().CODIGO : '';*/
                            data.ID = row.data().ID;
                            return data;
                        }
                    }
                },
                table: tableChild,
                fields: [
                    {
                        "label": "CONTENEDOR:",
                        "name": "DETCARGAB.IDEN",

                    },
                    {
                        "label": "BULTOS:",
                        "name": "DETCARGAB.BULTOS",
                        "attr": {
                            type: 'number'
                        }
                    },
                    {
                        "label": "PESO:",
                        "name": "DETCARGAB.PESO_ORIG",
                        "attr": {
                            type: 'number'
                        }
                    },
                    {
                        "label": "ID:",
                        "name": "DETCARGAB.ID_BOOKINGS_EXPO",
                        "type": "hidden"

                    }
                ],
                i18n: i18
            });
            btn_child.prop('disabled', true);
            row.child(tableChild).show();

            fetch(urlApi + 'Cntr/Get2', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'                   
                },
                body: JSON.stringify({
                    cn: cn_hash,
                    pkey: pkey,
                    nituser: nitUser,
                    isagent: isAgent,
                    Booking: booking,
                    ID: row.data().ID
                })
            })
                .then(function (response) {
                    if (!response.ok) {
                        throw new Error('Network response was not ok');
                    }

                    return response.json();
                })
                .then(function (data) {
                    btn_child.prop('disabled', false);
                    tableCntr = tableChild.DataTable({
                        dom: 'Bfrtip',
                        pageLength: 5,
                        data: data.data, // Los datos deben ser proporcionados aquí
                        columns: [
                            { title: 'Contenedor', data: 'DETCARGAB.IDEN' },
                            { title: 'Bultos', data: 'DETCARGAB.BULTOS' },
                            { title: 'Peso', data: 'DETCARGAB.PESO_ORIG' }
                        ],
                        select: true,
                        language: lang,
                        buttons: [
                            { extend: 'create', editor: editorCntr },
                            { extend: 'edit', editor: editorCntr },
                            { extend: 'remove', editor: editorCntr }
                        ]
                    });

                    // Agregar la tabla a la fila

                })
                .catch(function (error) {
                    console.error('Error:', error);
                    // Puedes manejar el error de otra manera, como mostrar un mensaje de error en la página.
                });

            editorCntr.on('onPreSubmit', function (e, data, action) {
                if (action === 'create') {
                    if (!/^\d+(\.\d+)?$/.test(data.data[0].DETCARGAB.BULTOS)) {
                        var numeroConComa = data.data[0]['PESONETO'];
                        var hola = 2;

                    }
                    else {
                        return 'hola'
                    }


                    console.log('Datos del nuevo registro:', data.data);
                }
            });
        }

        function destroyChild(row) {
            var table2 = $('table', row.child());
            table2.detach();
            table2.DataTable().destroy();

            row.child.hide();


        }

        booking = bl.trim();
        //tableFiles.on('select', function (e, data) {
        //    console.log(this.field('CODIGO'));
        //});

        $('#myModal').modal('show');
        $('#myModal').addClass('center-opening show');
        $('body').addClass('modal-open');

        $('#myModal').on('hidden.bs.modal', function () {
            $(this).removeClass('center-opening show');
            $('body').removeClass('modal-open');
        });
    }
</script>