package com.tipalti;

import com.sun.deploy.util.StringUtils;

import javax.crypto.Mac;
import javax.crypto.spec.SecretKeySpec;
import java.io.UnsupportedEncodingException;
import java.net.URLEncoder;
import java.util.*;

public class Main {

    public static void main(String[] args) {
        //string baseUrl = "https://ui.tipalti.com"; //production
        String baseUrl = "https://ui2.sandbox.tipalti.com"; //integration

        String payerName = "Demo1";
        String payeeId = "payeeToOpenHere"; //the unique identifier for the payee
        String privateKey = "FEVUEmSW41JnFY5GukJTM/l24usNpSQiuus3ySeoDGntOOGkqlBrp4Eu8tMrv506"; //The secret key received from Tipalti

        //any additional parameters you may want to pass, according to Tipalti's documentation. Passing unknown parameters will cause an error.
        Dictionary<String,String> additionalParams = new Hashtable<String, String>();
        additionalParams.put("first","John");
        additionalParams.put("last","Smith");

        String iframeUrl = CreateiFrameUrl(baseUrl, payerName, payeeId, privateKey, additionalParams);
        //Use this URL to open the iFrame
        System.out.println(iframeUrl);
    }

    public static String CreateiFrameUrl(String baseUrl, String payerName, String payeeId, String privateKey, Dictionary<String, String> parameters)
    {
        String queryString = CreateQueryString(payerName, payeeId, privateKey, parameters);

        String pageToOpen = "/Payees/PayeeDashboard.aspx"; //to open the payment details iFrame. Change this to open a different iframe such as Payment History.

        return String.format("%s%s?%s", baseUrl, pageToOpen, queryString);
    }

    public static String CreateQueryString(String payerName, String payeeId, String privateKey, Dictionary<String, String> parameters) {
        List<String> queryStringPairs = new ArrayList<String>();

        try {
            queryStringPairs.add(String.format("payer=%s", URLEncoder.encode(payerName, "UTF-8")));
            queryStringPairs.add(String.format("idap=%s", URLEncoder.encode(payeeId, "UTF-8")));
            //timestamp is seconds since Unix epoch in UTC timezone
            queryStringPairs.add(String.format("ts=%s", new Date().getTime() / 1000 ));

            for (String key : Collections.list(parameters.keys()))
            {
                String value = parameters.get(key);
                queryStringPairs.add(String.format("%s=%s", key, URLEncoder.encode(value, "UTF-8")));
            }

        } catch (UnsupportedEncodingException e) {
            e.printStackTrace();
        }

        String combinedQs = StringUtils.join(queryStringPairs,"&");

        String signature = EncryptFullQueryString(combinedQs, privateKey);
        combinedQs = combinedQs + "&hashkey=" + signature;

        return combinedQs;
    }

    public static String EncryptFullQueryString(String queryString, String secret)
    {
        try{
            Mac sha256_HMAC = Mac.getInstance("HmacSHA256");
            SecretKeySpec secret_key = new SecretKeySpec(secret.getBytes(), "HmacSHA256");
            sha256_HMAC.init(secret_key);

            String hash = hexencode(sha256_HMAC.doFinal(queryString.getBytes()));
            return hash;
        }
        catch (Exception e)
        {
            System.out.println("Error");
            return null;
        }
    }

    public static String hexencode(byte[] bytes){
        StringBuilder sb = new StringBuilder();
        for (byte b : bytes){
            sb.append(String.format("%02x",b));
        }
        return sb.toString();
    }
}
