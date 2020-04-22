$(() => {
    $("#add-contributor-btn").on("click", () => {
        $("#add-contributor-modal").modal();
    });

    $(".deposit-btn").on('click', function () {
        let id = $(this).data("contributor-id");
        $("#deposit-modal").modal();
        console.log(id);
        $("#contributor-id").attr("value", id);
    });

    $(".edit-btn").on('click', function () {
        let btn = $(this);
        let modal = `<div id="edit-contributor-modal-${btn.data("id")}" class="modal" tabindex="-1" role="dialog">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Add Simcha</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <form method="post" action="/home/EditContributor">
                    <div class="modal-body">
                        <input value="${btn.data("first-name")}" class="form-control" name="FirstName" placeholder="First Name" required style="margin:5px" />
                        <input value="${btn.data("last-name")}" class="form-control" name="LastName" placeholder="Last Name" required style="margin:5px" />
                        <input value="${btn.data("cell-number")}" class="form-control" name="CellNumber" placeholder="CellNumber" required style="margin:5px" />
                        <input `
        if (btn.data('always-include') === 'True') {
            modal += `checked`;
        }
        modal += ` type="checkbox" name="AlwaysInclude" value="true" />Always Include
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
                        <button class="btn btn-primary">Submit</button>
                    </div>
                </form>
            </div>
        </div>
    </div>
`;
        $("#contributors-page").append(modal);
        $(`#edit-contributor-modal-${btn.data("id")}`).modal();
    })
});