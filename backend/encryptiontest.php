<!DOCTYPE html>
<html>
<body>

<?php
ini_set( 'error_reporting', E_ALL );
ini_set( 'display_errors', true );
set_include_path(get_include_path() . PATH_SEPARATOR . 'phpseclib');
include('phpseclib/Crypt/RSA_XML.php');


function decrypt($data)
    {
        $rsaserver = new Crypt_RSA_XML();
        $myfile = fopen("privatekey.xml", "r") or die("Unable to open file!");
        $privatekey = fread($myfile,filesize("privatekey.xml"));
        $rsaserver->loadKey($privatekey);
        $result = $rsaserver->decrypt(base64_decode($data));
        return $result;
    }


function encrypt($data)
    {
        $rsaserver = new Crypt_RSA_XML();
        $myfile = fopen("publickey.xml", "r") or die("Unable to open file!");
        $publickey = fread($myfile,filesize("publickey.xml"));
        $rsaserver->loadKey($publickey);
        $result = $rsaserver->encrypt($data);
        return $result;
    }

$result = encrypt("S4xr5A1e45M8Z1X");


$test =  base64_encode($result);

// echo $test;
// echo decrypt(base64_decode(urldecode("AfI6gWL1euTC53nbvdi80B2W9j3yFoEU0ddaQrgN5VVIdvwgws89zxYTfHqb4LmjuzqhnQe8kcbvrvFotY3Jf%2BcvFseGkR8YBoH0ZrIRkAbF0wX1%2FQbtrbW48HuFq5FA%2FI%2BzZLhnnwpghEz9UgzYQubgS8YoEaDPXoWjulnfa2M%3D")));

echo decrypt($test);

?>


</body>
</html>
