# ScreenQ
Screenshot your bugs

With ScreenQ you can take screenshots and tag them directly without using any other application.
Press the "Print Screen" button in your keyboard while the program is running and an editor window will open with your screenshot.
When the Screenshot is saved, an Excel file will be written with the image, the tags you used and the additional comments you wrote.

Please, make sure to configure ScreenQ by right-clicking on the icon in your system tray before using it. You need to set the path where the screenshots are saved, as well as the path where your XML typology is stored.

## XML Typology
The typology must be a valid XML file like this:
### Header
```
<?xml version="1.0" encoding="UTF-8"?>
<screenq version="1">
    <head>
        <name>Name of your typology/name>
        <descrip>Description of your typology</descrip>
        <version>1.0</version>
        <src>URL (optional)</src>
    </head>
```

### Body
Currently, it supports up to 4 levels:
```
    <issueList>
      <category id="an unique ID">
        <categoryName>Level 1</categoryName>
        <categoryDesc>Level 1 description</categoryDesc>
        <issue id="an unique ID">
          <issueName>Level 2</issueName>
          <issueDesc>Level 2 description</issueDesc>
          <subIssue id="an unique ID">
            <subIssueName>Level 3</subIssueName>
            <subIssueDesc>Level 3 description</subIssueDesc>
            <subIssue2 id="an unique ID">
              <subIssue2Name>Level 4</subIssue2Name>
              <subIssue2Desc>Level 4 description</subIssue2Desc>
            </subIssue2>
          </subIssue>
        </issue>
      </category>
    </issueList>
</screenq>    
```

## FAQ
### How do I restart the counter for the screenshots?
This is currently not available from the UI, but you can go to 
`` C:\Users\artuv\AppData\Local\ScreenQ ``  and inside the folder with the random hash you will see another folder called 1.0.0.0, go inside and edit user.config as a text file, look for:
```           
<setting name="ScreenID" serializeAs="String">
  <value>XX</value>
 </setting>
 ```
 Change the number of the ``<value></value>`` to 0.
            
