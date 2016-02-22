import hmac
import hashlib
from time import time
from itertools import chain
from urllib import urlencode
from collections import OrderedDict


payer_name = 'Demo1'
payer_id = 'payeeToOpenHere'
private_key = 'FEVUEmSW41JnFY5GukJTM/l24usNpSQiuus3ySeoDGntOOGkqlBrp4Eu8tMrv506'


class Tipalti():
    def __init__(self, payer_name, payer_id, private_key, debug=False):
        self.base_url = 'https://ui2.sandbox.tipalti.com' if debug else \
            'https://ui2.tipalti.com'

        self.params = OrderedDict()
        self.params['payer'] = payer_name
        self.params['idap'] = payer_id
        self.private_key = private_key

        self.user_params = OrderedDict()

    def add_param(self, param, value):
        self.user_params[param] = value

    def get_url(self, page_to_open):
        self.params['ts'] = int(time())
        uri = '{0}%s?{1}' % page_to_open
        res = uri.format(self.base_url, self._query_string())
        self.user_params = OrderedDict()
        return res

    def _query_string(self):
        params = OrderedDict()
        chn = chain(self.params.iteritems(), self.user_params.iteritems())
        for param, value in chn:
            params[param] = value
        hash_key = self._hash(urlencode(params), self.private_key)
        return urlencode(OrderedDict(params, hashkey=hash_key))

    @staticmethod
    def _hash(value, secret):
        return hmac.new(secret, value, digestmod=hashlib.sha256).hexdigest()


def main():
    client = Tipalti(payer_name, payer_id, private_key, debug=True)
    client.add_param('first', 'John')
    client.add_param('last', 'Smith')
    url = client.get_url('/Payees/PayeeDashboard.aspx')
    print url

if __name__ == '__main__':
    main()
