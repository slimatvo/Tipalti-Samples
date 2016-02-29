<?php

echo build_iframe_url("Demo1", "payeeToOpenHere", "FEVUEmSW41JnFY5GukJTM/l24usNpSQiuus3ySeoDGntOOGkqlBrp4Eu8tMrv506", ["first" => "John", "last" => "Smith"]);

function build_iframe_url($payer_name, $payee_id, $secret_key, $parameters = null)
{
	//$baseURL = "https://ui.tipalti.com"; //production
	$baseURL = "https://ui2.sandbox.tipalti.com"; //sandbox
	
	return $baseURL."/Payees/PayeeDashboard.aspx?".build_query_string($payer_name, $payee_id, $secret_key, $parameters);
}

function build_query_string($payer_name, $payee_id, $secret_key, $parameters = null)
{
	$timestamp = time(); //time is number of seconds since Unix Epoch, in UTC
	$qs_parts = [
		"payer" => urlencode($payer_name),
		"idap" => urlencode($payee_id), 
		"ts" => urlencode($timestamp)
	];
	
	//add the additional parameters
	if ($parameters != null)
	{
		foreach ($parameters as $k => $v)
		{
			$qs_parts[urlencode($k)] = urlencode($v);
		}
	}
	
	$qsArr = [];
	foreach($qs_parts as $k=>$v)
	{
		array_push($qsArr, $k . "=" . $v);
	}
	
	$qs = implode("&",$qsArr);
	
	$qs = $qs . "&hashkey=" . encrypt_query_string($qs,$secret_key);
	
	return $qs;
}

function encrypt_query_string($query_string, $secret_key)
{
	return hash_hmac("sha256",$query_string,$secret_key);
}

?>
