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
var imgAvatarRemove = document.getElementById('imgAvatarRemove');
var selNewPerson = document.getElementById('selNewPerson');
var request;
var iMovieID = '';

var queryParams = (new URL(document.location)).searchParams;
var iMovieQueryParam = queryParams.get("MovieID");
if (iMovieQueryParam)
    iMovieID = iMovieQueryParam;

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
                elPerson.setAttribute('data-id', item.PersonID.toString());
                elPerson.addEventListener('click', searchListItemClicked);
                searchbarList.appendChild(elPerson);
            });
        },
        error: function (res) {
            console.log(res);
        }
    });
});

imgAvatarRemove.addEventListener('click', event => {
    fileImage.value = '';
    event.target.previousElementSibling.src = '/images/movie_noimage.png';
})

$('#formAddPerson').submit(event => {
    if (!$('#formAddPerson').valid())
        return;
    event.preventDefault();

    var form = event.target;
    var formControls = $(form).find('input,textarea,select');
    var dataObj = {};

    $(form).serializeArray().map(function (x) {
        var key = x.name;
        if (x.name.includes("NewPerson"))
            key = key.replace("NewPerson.", "");

        dataObj[key] = x.value;
    });

    if(iMovieID)
        dataObj.MovieID = iMovieID;
    
    dataObj.RoleReferenceID = selNewPerson.value;

    $.ajax({
        url: form.getAttribute('action'),
        method: form.getAttribute('method'),
        dataType: "json",
        data: dataObj,
        beforeSend: function () {
            formControls.prop('disabled', true);
        },
        success: function (oPerson) {
            CreateTagForPerson(oPerson.PersonID, selNewPerson.value, oPerson.Name);
        },
        error: function (res, textStatus, ex) {
            console.log("failed");
        },
        complete: function () {
            formControls.prop('disabled', false);
        }
    });
});

function searchListItemClicked(event){
    var personListItem = event.target;
    if (!personListItem)
        return;

    var iRoleReferenceID = document.querySelector("input[name='rbtnSearchRole']:checked").value;
    var iPersonID = personListItem.getAttribute('data-id');

    var bTagCreated = CreateTagForPerson(iPersonID, iRoleReferenceID, personListItem.innerText);

    if(bTagCreated && iMovieID)
        AddRoleToMovie(iPersonID,iMovieID,iRoleReferenceID);
}

function AddRoleToMovie(iPersonID, iMovieID, iRoleReferenceID){

    var dataObj = {"PersonID":iPersonID,"MovieID":iMovieID,"RoleReferenceID":iRoleReferenceID};
    var forgTk = document.querySelector("input[name='__RequestVerificationToken']").value;

    $.ajax({
        url:'/Person/AddRoleToMovie',
        method:'POST',  
        headers: {
            'RequestVerificationToken':forgTk,
        },
        data:dataObj,
        success:function(){
            console.log('success');
        },
        error:function(){
            console.log('error');
        }
    })
}

function CreateTagForPerson(iPersonID, iRoleReferenceID, strName) {

    var bPersonExists = CheckIfPersonExists(iRoleReferenceID, iPersonID);

    if (bPersonExists)
        return;

    var listElmnt = document.createElement('li');
    listElmnt.innerText = strName;

    var inputElmnt = document.createElement('input');
    inputElmnt.type = 'hidden';
    inputElmnt.setAttribute('data-id', iPersonID);
    inputElmnt.value = iPersonID;

    var closeBtn = document.createElement('i');
    closeBtn.innerText = 'x';
    closeBtn.addEventListener('click', closeBtnClicked);

    listElmnt.appendChild(inputElmnt);
    listElmnt.appendChild(closeBtn);

    var targetList = getTargetList(iRoleReferenceID);

    if (!targetList)
        return;

    listElmnt.setAttribute('data-roleReferenceID', iRoleReferenceID);
    targetList.appendChild(listElmnt);

    createNameAttribute(targetList, targetList.getAttribute('data-listType'));

    return true;
}

function CheckIfPersonExists(roleReferenceID, iPersonID) {
    var targetList = getTargetList(roleReferenceID);

    if (!targetList)
        return;

    if (targetList.getElementsByTagName('input').length == 0)
        return;

    var arrTargetList = Array.from(targetList.getElementsByTagName('input'));

    return arrTargetList.filter(item => {
        return item.getAttribute('data-id') == iPersonID;
    }).length;
}

function closeBtnClicked(event) {
    var listItem = event.target.parentNode;
    var iRoleReferenceID = listItem.getAttribute('data-roleReferenceID');
    RemovePerson(listItem, iRoleReferenceID);
    var iPersonID = listItem.children[0].getAttribute('data-id');

    if(iMovieID)
        RemoveRoleFromMovie(iPersonID,iMovieID,iRoleReferenceID);
}

function RemovePerson(listItem, roleReferenceID) {
    var targetList = getTargetList(roleReferenceID);

    if (!targetList)
        return;

    targetList.removeChild(listItem);

    createNameAttribute(targetList, targetList.getAttribute('data-listType'));
}

function RemoveRoleFromMovie(iPersonID, iMovieID, iRoleReferenceID){

    var dataObj = {"PersonID":iPersonID,"MovieID":iMovieID,"RoleReferenceID":iRoleReferenceID};
    var forgTk = document.querySelector("input[name='__RequestVerificationToken']").value;

    $.ajax({
        url:'/Person/RemoveRoleFromMovie',
        method:'POST',
        headers:{
            'RequestVerificationToken':forgTk
        },
        data:dataObj,
        success:function(){
            console.log('success');
        },
        error:function(){
            console.log('error');
        }
    });
}

function getTargetList(roleReferenceID) {
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
    return targetList;
}

function createNameAttribute(targetList, listType) {
    targetList = Array.from(targetList.getElementsByTagName('input'));
    targetList.forEach((item, i) => {
        item.setAttribute('name', `${listType}[${i}]`);
    })
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