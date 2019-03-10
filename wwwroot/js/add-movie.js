$('#dtYearOfRelease').flatpickr({
    dateFormat: "d-m-Y",
});
$('#dtDateOfBirth').flatpickr({
    dateFormat: "d-m-Y",
});

var fileImage = document.getElementById('fileImage');
var imgAvatar = document.getElementById('imgAvatar');
var txtSearchBar = document.getElementById('txtSearchBar');
var searchbarList = document.getElementById('searchbarList');
var selectedActorsContainer = document.getElementById('selectedActorsContainer');
var selectedProducersContainer = document.getElementById('selectedProducersContainer');
var rbtnSearchRoleActor = document.querySelectorAll("input[name='rbtnSearchRole']")[0];
var request;

fileImage.addEventListener('change', () => {
    applyImageToAvatar(fileImage);
});

txtSearchBar.addEventListener('keyup', async event => {
    var iMinimumLetters = 3;

    if (txtSearchBar.value.length < iMinimumLetters)
        return;

    if (request)
        request.abort();

    var forgTk = document.querySelector("input[name='__RequestVerificationToken']").value;

    request = $.ajax({
        url: `/Person/SearchPerson?Name=${txtSearchBar.value}`,
        type: 'GET',
        headers: {
            "RequestVerificationToken": forgTk
        },
        contentType: "application/json; charset=utf-8",
        dataType: 'json',
        beforeSend: function () {
            searchbarList.innerHTML = '';
        },
        success: function (resPeople) {

            var arrPeople = resPeople;
            arrPeople.forEach(item => {
                var elPerson = document.createElement('li');
                elPerson.innerText = item.Name;
                elPerson.setAttribute('data-id', item.PersonID);
                elPerson.addEventListener('click', CreateTagForPerson);

                searchbarList.appendChild(elPerson);
            });
        },
        error: function (res) {
            console.log(res);
        }
    });
});

function CreateTagForPerson(event) {

    var personListItem = event.target;

    if (!personListItem)
        return;

    var iRoleReferenceID = document.querySelector("input[name='rbtnSearchRole']:checked").value;

    var bPersonExists = CheckIfPersonExists(iRoleReferenceID, personListItem.getAttribute('data-id'));

    if (bPersonExists)
        return;

    var listElmnt = document.createElement('li');
    listElmnt.innerText = personListItem.innerText;

    var inputElmnt = document.createElement('input');
    inputElmnt.type = 'hidden';
    inputElmnt.setAttribute('data-id', personListItem.getAttribute('data-id'));
    inputElmnt.value = personListItem.getAttribute('data-id');

    var closeBtn = document.createElement('i');
    closeBtn.innerText = 'x';
    closeBtn.addEventListener('click', closeBtnClicked);

    listElmnt.appendChild(inputElmnt);
    listElmnt.appendChild(closeBtn);

    if (rbtnSearchRoleActor.checked) {

        var listCount = selectedActorsContainer.getElementsByTagName('li').length;

        listElmnt.setAttribute('data-roleReferenceID', 1);
        inputElmnt.name = `Actors[${listCount}].PersonID`;
        selectedActorsContainer.appendChild(listElmnt);
    }
    else {

        var listCount = selectedProducersContainer.getElementsByTagName('li').length;

        listElmnt.setAttribute('data-roleReferenceID', 2);
        inputElmnt.name = `Producers[${listCount}].PersonID`;
        selectedProducersContainer.appendChild(listElmnt);
    }
}

function CheckIfPersonExists(roleReferenceID, iPersonID) {
    var targetList;

    switch (roleReferenceID) {
        case "1":
            targetList = selectedActorsContainer;
            break;
        case "2":
            targetList = selectedProducersContainer;
            break;
        default:
            break;
    }

    if (targetList.getElementsByTagName('input').length == 0)
        return;

    var arrTargetList = Array.from(targetList.getElementsByTagName('input'));

    return arrTargetList.filter(item => {
        return item.getAttribute('data-id') == iPersonID;
    }).length;
}

function closeBtnClicked(event) {
    var listItem = event.target.parentNode;
    var roleReferenceID = listItem.getAttribute('data-roleReferenceID');
    RemovePerson(listItem, roleReferenceID)
}

function RemovePerson(listItem, roleReferenceID) {
    switch (roleReferenceID) {
        case "1":
            selectedActorsContainer.removeChild(listItem);
            break;

        case "2":
            selectedProducersContainer.removeChild(listItem);
            break;

        default:
            break;
    }
}

function applyImageToAvatar(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = e => {
            imgAvatar.src = e.target.result;
        };

        reader.readAsDataURL(input.files[0]);
    }
}