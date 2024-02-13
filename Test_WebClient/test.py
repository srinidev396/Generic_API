import httpx
import asyncio
import unittest
import openai

url = "https://restapi.tabfusionrms.com"
msg = ""
token = ""

async def Auth():
    global token
    rdata = ""
    requestData = "json"
    userName = "administrator"
    password = 'password$'
    database = "QaData"

    linkurl = f"{url}/GenerateToken?userName={userName}&passWord={password}&database={database}"
    async with httpx.AsyncClient() as client:
        response = await client.get(linkurl)
        if response.status_code != 200:
            return response
        token = response.json()["token"]
        return token

async def NewRecord():
    api = "Data/NewRecord"
    data = {
        "tableName": "boxes",
        "postRow": [
                {
                    "value": "48",
                    "columnName": "Description"
                },
                {
                    "value": "moti test",
                    "columnName": "OffSiteNo"
                }
        ]   
    }
    await FETCHPOST(api, data)

# Define other functions similarly

async def GetUserViews():
    api = "Data/GetUserViews"
    await FETCHGET(api)

async def GetDbSchema():
    api = "Data/GetDbSchema"
    await FETCHGET(api)

async def GetTableSchema():
    tableName = "Boxes"
    api = f"Data/GetTableSchema?TableName={tableName}"
    await FETCHGET(api)

async def GetViewData():
    viewid = 45
    pageNumber = 2
    api = f"Data/GetViewData?viewid={viewid}&pageNumber={pageNumber}"
    await FETCHGET(api)

async def FETCHPOST(api, data):
    linkurl = f"{url}/{api}"
    async with httpx.AsyncClient() as client:
        response = await client.post(linkurl, json=data, headers={
            'Content-Type': 'application/json',
            'Authorization': f'Bearer {token}',
        })
        if response.status_code != 200:
            print(response)
        else:
            print(response.json()) 

async def FETCHGET(api):
    linkurl = f"{url}/{api}"
    async with httpx.AsyncClient() as client:
        response = await client.get(linkurl, headers={
            'Authorization': f'Bearer {token}',
        })
        if response.status_code != 200:
            print(response)
        else:
            print(response.json())

    

def add(a, b):
    return a + b

class TestAPI(unittest.TestCase):
 def check_token(self):
        Auth()
        self.assertEquals(token, '')

"""     def test_add_negative_numbers(self):
       result = add(3, 3)
       self.assertEqual(result, 6) """
    
    


if __name__ == '__main__':
    #asyncio.run(Auth())
    unittest.main()
    #asyncio.run(NewRecord())