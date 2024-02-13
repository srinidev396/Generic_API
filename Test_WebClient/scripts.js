//var url = "http://localhost:2234";
var url = "https://restapi.tabfusionrms.com";

var msg = document.getElementById("msgid");
//authenticate
async function Auth() {
    var rdata = ""
    var requestData = "json"
    var userName = "administrator";
    var password = 'password$';
    var database = "QaData";

    var linkurl = `${url}/GenerateToken?userName=${userName}&passWord=${password}&database=${database}`;
    const call = await fetch(`${linkurl}`);
    if (requestData === "html")
        rdata = await call.text();
    else if (requestData === "json")
        rdata = await call.json();
    localStorage.setItem("token", rdata.token);
    msg.innerHTML = `token created: ${rdata.token} you can use any function`;
}


function NewRecord() {
    api = "Data/NewRecord";
    var data = {
        "tableName": "boxes",
        "postRow": [
            [
                {
                    "value": "48",
                    "columnName": "Descsription"
                },
                {
                    "value": "1",
                    "columnName": "yesno"
                },
                {
                    "value": "moti test",
                    "columnName": "OffSiteNo"
                }
            ]
        ]
    }
    FETCHPOST(api, data).then((data) => {
        msg.innerHTML = JSON.stringify(data);
    }).catch((data) => {
        msg.innerHTML = JSON.stringify(data);
    })
}


//software entery.
//you call fusion api

function NewRecordMulti() {
    api = "Data/NewRecordMulti";
    var row = [];
    var rows = [];
    for (let i = 0; i < 10; i++) {
        var x = i + 1;
        row.push({ columnName: "Description", value: `<dec${i}>` })
        row.push({ columnName: "OffSiteNo", value: `<off${i}>` })
        row.push({ columnName: "yesno", value: `1` })
        row.push({ columnName: "IntegerField", value: `1` })
        rows.push(row);
        row = [];
    }
    var data = {
        "tableName": "Boxes",
        "postMultiRows": rows
    }

    FETCHPOST(api, data).then((data) => {
        msg.innerHTML = JSON.stringify(data);
    }).catch((data) => {
        msg.innerHTML = JSON.stringify(data);
    })
}


function EditRecord() {
    api = "Data/EditRecord";

    var data = {
        "tableName": "Boxes",
        "keyValue": "70645",
        "fieldName": "id",
        "postRow": [
            {
                "value": "3333",
                "columnName": "IntegerField",
            },
            {
                "value": "helooffsetno",
                "columnName": "OffSiteNo"
            }
        ],
    }

    FETCHPOST(api, data).then((data) => {
        msg.innerHTML = JSON.stringify(data);
    }).catch((data) => {
        msg.innerHTML = JSON.stringify(data);
    })
}

function EditRecordByColumn() {
    api = "Data/EditRecordByColumn";

    var data = {
        "tableName": "Boxes",
        "keyValue": "null",
        "fieldName": "memofield",
        "isMultyupdate": true,
        "postRow": [
            {
                "value": "Hello11",
                "columnName": "OffSiteNo",
            },
            {
                "value": "Hello22",
                "columnName": "Description",
            }
        ],
    }

    FETCHPOST(api, data).then((data) => {
        msg.innerHTML = JSON.stringify(data);
    }).catch((data) => {
        msg.innerHTML = JSON.stringify(data);
    })
}

function EditIfnotExistAdd() {
    api = "Data/EditIfNotExistAdd";

      var data = {
        "tableName": "Boxes",
        "fieldName": "Description",
        "keyValue": "value-exist",
        "isMultyupdate": true,
        "postRow": [ //this is a test:
            {
                "value": "1",
                "columnName": "Description"
            }
        ]

    }

    FETCHPOST(api, data).then((data) => {
        msg.innerHTML = JSON.stringify(data);
    }).catch((data) => {
        msg.innerHTML = JSON.stringify(data);
    })
}

function GetUserViews() {
    var api = "Data/GetUserViews";
    FETCHGET(api).then((data) => {
        msg.innerHTML = JSON.stringify(data);
    })
}

function GetDbSchema() {
    var api = `Data/GetDbSchema`;
    FETCHGET(api).then((data) => {
        msg.innerHTML = JSON.stringify(data);
    })
}

function GetTableSchema() {
    var tableName = "Boxes";
    var api = `Data/GetTableSchema?TableName=${tableName}`;
    FETCHGET(api).then((data) => {
        msg.innerHTML = JSON.stringify(data);
    })
}

function GetViewData() {
    var viewid = 45;
    var pageNumber = 2;
    var api = `Data/GetViewData?viewid=${viewid}&pageNumber=${pageNumber}`
    FETCHGET(api).then((data) => {
        msg.innerHTML = JSON.stringify(data);
    });
}


async function FETCHPOST(api, data) {
    console.log(JSON.stringify(data))
    msg.innerHTML = "Working on it.........";
    var linkurl = `${url}/${api}`;
    const response = await fetch(linkurl, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${localStorage.getItem("token")}`,
        },
        body: JSON.stringify(data)
    })
    if (response.ok == false) {
        return response
    } else {
        return response.json();
    }
}

async function FETCHGET(api) {
    msg.innerHTML = "Working on it.........";
    var linkurl = `${url}/${api}`;
    var token = `Bearer ${localStorage.getItem("token")}`
    const call = await fetch(linkurl, {
        method: 'GET',
        headers: {
            'Authorization': token,
        },
    });
    if (call.ok == false) {
        return call;
    } else {
        return await call.json();
    }
}

