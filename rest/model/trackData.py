
class TrackData:
    id = None
    name = ''

    # if both are set, local_file is preferred
    local_file = None
    uri = None

    
    image_uri = None
    description = ''
    author = ''
    album = ''
    genre = ''
    duration = ''



    def __init__(self, raw_input = None):

        # input from database
        if isinstance(raw_input, tuple):
            self.parse_from_tuple(raw_input)

        # input from rest
        elif isinstance(raw_input, dict):
            self.parse_from_json(raw_input)




    def parse_from_tuple(self, db_tuple):
        self.id = db_tuple[0]
        self.name = db_tuple[1]
        self.local_file = db_tuple[2]
        self.uri = db_tuple[3]
        self.image_uri = db_tuple[4]
        self.description = db_tuple[5]
        self.author = db_tuple[6]
        self.album = db_tuple[7]
        self.genre = db_tuple[8]
        self.duration = db_tuple[9]


    def parse_from_json(self, db_dict):
        self.id = db_dict.get('id', None)
        self.name = db_dict.get('name', '')
        self.local_file = db_dict.get('local_file', None)
        self.uri = db_dict.get('uri', None)
        self.image_uri = db_dict.get('image_uri', None)
        self.description = db_dict.get('description', '')
        self.author = db_dict.get('author', '')
        self.album = db_dict.get('album', '')
        self.genre = db_dict.get('genre', '')
        self.duration = db_dict.get('duration', '')



    def to_json(self):
        return dict({
            'id': self.id,
            'name': self.name,
            'local_file': self.local_file,
            'uri': self.uri,
            'image_uri': self.image_uri,
            'description': self.description,
            'author': self.author,
            'album': self.album,
            'genre': self.genre,
            'duration': self.duration
        })