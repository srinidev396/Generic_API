var url = "http://localhost:2234";
var msg = document.getElementById("msgid");
//authenticate
async function Auth() {
    var rdata = ""
    var requestData = "json"
    var linkurl = `${url}/GenerateToken?userName=administrator&passWord=password$&database=cfg`;
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
        "tableName": "Documents",
        "postRow": [
            {
                "value": "48",
                "columnName": "DocumentID"
            },
            {
                "value": "new data",
                "columnName": "DocumentName"
            }
        ]
    }
    FetchPost(api, data).then((data) => {
        msg.innerHTML = JSON.stringify(data);
    }).catch((data) => {
        msg.innerHTML = JSON.stringify(data);
    })
}

function NewRecordMulti() {
    api = "Data/NewRecordMulti";
    var row = [];
    var rows = [];
    for (let i = 0; i < 10000; i++) {
        var x = i + 1;
        row.push({ columnName: "Id", value: `id${i}` })
        row.push({ columnName: "Description", value: `<dec${i}>` })
        row.push({ columnName: "Out", value: `0` })
        rows.push(row);
        row = [];
    }
    /* rows = [
        [
            { "Value": "1111", "ColumnName": "Id" },
            { "Value": "hello", "ColumnName": "Description" },
            { "Value": "0", "ColumnName": "Out" },
        ],
        [
            { "Value": "1112", "ColumnName": "Id" },
            { "Value": "hello", "ColumnName": "Description" },
            { "Value": "0", "ColumnName": "Out" },
        ],
        [
            { "Value": "1113", "ColumnName": "Id" },
            { "Value": "hello", "ColumnName": "Description" },
            { "Value": "0", "ColumnName": "Out" },
        ]
    ] */
    var data = {
        "tableName": "Boxes",
        "postMultiRows": rows
    }

    FetchPost(api, data).then((data) => {
        msg.innerHTML = JSON.stringify(data);
    }).catch((data) => {
        msg.innerHTML = JSON.stringify(data);
    })
}


function EditRecord() {
    api = "Data/EditRecord";

    var data = {
        "tableName": "Documents",
        "keyValue": "100",
        "fieldName": "DocumentID",
        "postRow": [
            {
                "value": "editrecord",
                "columnName": "DocumentName",
            }
        ],
    }

    FetchPost(api, data).then((data) => {
        msg.innerHTML = JSON.stringify(data);
    }).catch((data) => {
        msg.innerHTML = JSON.stringify(data);
    })
}

function EditRecordByColumn() {
    api = "Data/EditRecordByColumn";

    var data = {
        "tableName": "Documents",
        "keyValue": "moti",
        "fieldName": "DocumentName",
        "isMultyupdate": true,
        "postRow": [
            {
                "value": "hello moti",
                "columnName": "DocumentName",
            }
        ],
    }

    FetchPost(api, data).then((data) => {
        msg.innerHTML = JSON.stringify(data);
    }).catch((data) => {
        msg.innerHTML = JSON.stringify(data);
    })
}

function EditIfnotExistAdd() {
    api = "Data/EditIfNotExistAdd";

    var data = {
        "tableName": "Documents",
        "keyValue": "101",
        "fieldName": "DocumentID",
        "postRow": [
            {
                "value": "101",
                "columnName": "DocumentID"
            },
            {
                "value": "<script>adfads",
                "columnName": "DocumentName"
            }
        ],
    }

    FetchPost(api, data).then((data) => {
        msg.innerHTML = JSON.stringify(data);
    }).catch((data) => {
        msg.innerHTML = JSON.stringify(data);
    })
}


async function FetchPost(api, data) {
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


