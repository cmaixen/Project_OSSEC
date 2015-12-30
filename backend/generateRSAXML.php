<!DOCTYPE html>
<html>
<body>
<?php


ini_set( 'error_reporting', E_ALL );
ini_set( 'display_errors', true );
set_include_path(get_include_path() . PATH_SEPARATOR . 'phpseclib');
include('phpseclib/Crypt/RSA_XML.php');

$privatekey_file = "Privatekey.xml";
$publickey_file = "Publickey.xml";
$rsa = new Crypt_RSA_XML();
extract($rsa->createKey());

//save privatekey
$fh = fopen($privatekey_file, 'w');
fwrite($fh, $privatekey);
fclose($fh);

//save publickey
$fh = fopen($publickey_file, 'w');
fwrite($fh, $publickey);
fclose($fh);

echo $privatekey;
echo nl2br($publickey);

?>


</body>
</html>