require 'openssl'
require 'Base64'
require 'URI'

def build_iframe_url(payer,payee_id,secret_key,parameters = nil)
	#base_url = "https://ui.tipalti.com" #production
	base_url = "https://ui2.sandbox.tipalti.com" #sandbox
	
	base_url + "/Payees/PayeeDashboard.aspx?" + build_query_string(payer,payee_id,secret_key,parameters)
end

def build_query_string(payer,payee_id,secret_key,parameters = nil)
	ts = Time.now.to_i #timestamp is seconds since Unix epoch in seconds.
	
	query_pairs = Hash["payer" => payer, "idap" => payee_id, "ts" => ts.to_s]
	
	#add the optional parameters
	parameters.each {|k,v| query_pairs[k] = v } unless parameters == nil
	
	qs = query_pairs.map { |k,v| URI.encode(k) + "=" + URI.encode(v)}.join("&")
	
	qs = qs + "&hashkey=" + encrypt_query_string(qs,secret_key)
end

def encrypt_query_string(qs, secret)	
	#hmac256  -> convert to hex -> add leading 0 to values below 0x10
	OpenSSL::HMAC.digest(OpenSSL::Digest.new('sha256'), secret, qs).each_byte.map { |b| b.to_s(16).rjust(2, '0') }.join
end

print build_iframe_url("Demo1", "payeeToOpenHere", "FEVUEmSW41JnFY5GukJTM/l24usNpSQiuus3ySeoDGntOOGkqlBrp4Eu8tMrv506", Hash["first" => "John", "last" => "Smith"])
