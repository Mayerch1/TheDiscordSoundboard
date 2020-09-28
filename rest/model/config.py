class Config:
    id = None
    
    bot_token = ''

    bot_volume = 1.0
    bot_owner_id = 0

    button_width = 240
    button_height = 240


    def __init__(self, raw_input = None):
         # input from database
        if isinstance(raw_input, tuple):
            self.parse_from_tuple(raw_input)

        # input from rest
        elif isinstance(raw_input, dict):
            self.parse_from_json(raw_input)
            pass


    def parse_from_tuple(self, db_tuple):
        self.id = db_tuple[0]
        self.bot_token = db_tuple[1]
        self.bot_volume = db_tuple[2]
        self.bot_owner_id = db_tuple[3]
        self.button_width = db_tuple[4]
        self.button_height = db_tuple[5]



    def parse_from_json(self, db_dict):
        self.id = db_dict.get('id', None)
        self.bot_token = db_dict.get('bot_token', '')
        self.bot_volume = db_dict.get('volume', 1.0)
        self.bot_owner_id = db_dict.get('bot_owner_id', 0)
        self.button_width = db_dict.get('button_width', 240)
        self.button_height = db_dict.get('button_height', 240)


    def to_json(self):
        d = dict({
            'id': self.id,
            'bot_token': self.bot_token,
            'volume': self.bot_volume,
            'bot_owner_id': self.bot_owner_id,
            'button_width': self.button_width,
            'button_height': self.button_height
        })

        return d