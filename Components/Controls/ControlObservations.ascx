<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ControlObservations.ascx.cs" Inherits="AllNet.Modules.ReservasExportaciones.Components.Controls.ControlObservations" %>
<div class="modal fade " id="CommentsModal" tabindex="-1" role="dialog" data-backdrop="static" aria-labelledby="myModalLabel">
    <div class="modal-dialog modal-lg modal-vertical-centered modal-dialog-custom modal-lg-custom" role="document">
        <div class="modal-content modal-content-custom">
            <div class="modal-header">
                <h2 class="modal-title text-center" id="myModalLabel2">Comentarios</h2>
                <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body tabla-contenedor" id="modal-body2">
                <div class="container-comments">
                    <div class="comments">
                        <div class="tabla-comentarios-tbl">
                            <table cellpadding="0" cellspacing="0" border="0" class="table table-striped table-bordered display align-middle text-center" id="tbl_comments" width="100%">
                                <thead>
                                </thead>
                            </table>
                        </div>
                    </div>
                    <div class="comments">
                        <div class="cont-boxcomment">
                            <input type="hidden" id="bl_orig">
                            <textarea id="comentario" name="comentario" required></textarea>
                        </div>
                        <div class="cont-btn">
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
            serverSide: true,
            processing: true,
            destroy: true,
            order: [1, 'asc'],
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
                {
                    title: "ID_COMMENT",
                    data: "ID_COMMENT",
                    visible: false,
                    key: true
                },
                {
                    title: "Reserva",
                    visible: false,
                    data: "BL_ORIG"
                },
                {
                    title: "Observación",
                    data: "COMMENT"
                },
                {
                    title: "Fecha",
                    data: "COMMENT_DATE",
                    defaultContent: ""
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
                    isagent: isAgent,
                    Comment: comment
                },
                type: 'POST',
                success: function (response) {
                    console.log(response);
                    alert("Comentario agregado");
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
