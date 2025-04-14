<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ObservationsControl.ascx.cs" Inherits="AllNet.Modules.ReservasExportaciones.Components.Controls.ObservationsControl" %>
<style>
    .modal-dialog {
        max-width: 900px;
        margin: 30px auto;
    }

    .modal-content {
        border-radius: 10px;
        overflow: hidden;
    }

    .modal-body {
        max-height: 60vh;
        overflow-y: auto;
        padding: 20px;
    }

    .container-comments {
        display: flex;
        flex-direction: column;
        gap: 20px;
    }

    .tabla-comentarios-tbl {
        overflow-x: auto;
        border: 1px solid #ddd;
        padding: 10px;
        border-radius: 6px;
        background-color: #f9f9f9;
    }

    .cont-boxcomment textarea {
        width: 100%;
        min-height: 80px;
        border-radius: 6px;
        border: 1px solid #ccc;
        padding: 10px;
        resize: vertical;
        font-size: 14px;
    }

    .cont-btn {
        display: flex;
        justify-content: flex-end;
    }

    #tbl_comments {
        width: 100% !important;
        display: block;
    }
</style>
<div class="modal fade" id="CommentsModal" tabindex="-1" role="dialog" data-backdrop="static" aria-labelledby="myModalLabel">
  <div class="modal-dialog modal-lg modal-vertical-centered modal-dialog-custom modal-lg-custom" role="document">
    <div class="modal-content modal-content-custom">
      <div class="modal-header">
        <h2 class="modal-title text-center" id="myModalLabel2">Comentarios</h2>
        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
          <span aria-hidden="true">&times;</span>
        </button>
      </div>

      <!-- Cuerpo del modal con scroll controlado -->
      <div class="modal-body tabla-contenedor" id="modal-body2">
        <div class="container-comments">
          <div class="comments">
            <!-- Contenedor con scroll horizontal para la tabla -->
            <div class="tabla-comentarios-tbl">
              <table cellpadding="0" cellspacing="0" border="0"
                     class="table table-striped table-bordered display nowrap align-middle text-center"
                     id="tbl_comments" width="100%">
                <thead>                 
                </thead>         
              </table>
            </div>
          </div>

          <div class="comments mt-3">
            <div class="cont-boxcomment">
              <input type="hidden" id="bl_orig">
              <textarea id="comentario" name="comentario" required class="form-control" placeholder="Escribe tu comentario..."></textarea>
            </div>
            <div class="cont-btn mt-2">
              <button type="button" id="btn-admin" class="btn btn-primary" onclick="AgregarComentario()">Agregar comentario</button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</div>
<script>
    function VerObservaciones(bl) {    
        $('#CommentsModal').modal('show');
        $('#CommentsModal').addClass('center-opening show');
        $('body').addClass('modal-open');


        $('#CommentsModal').on('hidden.bs.modal', function () {
            $(this).removeClass('center-opening show');
            $('body').removeClass('modal-open');
        });
        document.getElementById("bl_orig").value = bl
        tableComments = $('#tbl_comments').DataTable({
            dom: 'Bfrtip',
            scrollY: '300px',
            scrollCollapse: true,
            pageLength: 4,
            destroy: true,
            processing: true,
            order: [4, 'desc'], 
            ajax: {
                url: urlApi + 'Comments/Get',
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
                { title: "ID_COMMENT", data: "ID_COMMENT", visible: false, key: true },
                { title: "Reserva", data: "BL_ORIG", visible: false },
                { title: "Observación", data: "COMMENT" },
                { title: "Usuario", data: "USER" },
                { title: "Fecha", data: "COMMENT_DATE", defaultContent: "" }
            ],
            columnDefs: [
                {
                    targets: 4, // Índice de la columna de fecha
                    render: function (data, type, row) {
                        if (!data) return '';

                        // Separar por espacio y luego por / y :
                        const [datePart, timePart] = data.split(' ');
                        const [day, month, year] = datePart.split('/');
                        const [hour = 0, minute = 0, second = 0] = timePart ? timePart.split(':') : [0, 0, 0];

                        const dateObj = new Date(
                            parseInt(year),
                            parseInt(month) - 1,
                            parseInt(day),
                            parseInt(hour),
                            parseInt(minute),
                            parseInt(second)
                        );

                        if (type === 'display' || type === 'filter') {
                            return data; // mostrar formato original
                        }

                        return dateObj.getTime(); // para ordenar correctamente incluso por segundos
                    }
                }
            ],
            select: {
                style: 'os',
                selector: 'td:not(:first-child)'
            },
            lengthChange: false,
            language: lang
        });
    }



    function AgregarComentario() {
        var bl = document.getElementById("bl_orig").value;
        var user1 = "UserTest";
        var comment = document.getElementById("comentario").value;
        if (comment.trim() != "") {
            $.ajax({
                url: urlApi + 'Comments/InsertComment',
                beforeSend: sf.setModuleHeaders,
                data: {
                    cn: cn_hash,
                    pkey: pkey,
                    Booking: bl,
                    nituser: nitUser,
                    user1: user1,
                    isagent: isAgent,
                    Comment: comment
                },
                type: 'POST',
                success: function (response) {
                    console.log(response);
                    tableComments.ajax.reload();     
                },
                error: function (error) {
                    console.error(error);
                }
            });
            tableComments.ajax.reload();
            document.getElementById("comentario").value = "";
        }
    }  
</script>
