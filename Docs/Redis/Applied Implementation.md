# Redis Implementation in AABC

Redis is an in-memory key-value store. Redis maps keys to types of values. An important difference between Redis and other structured storage systems is that Redis supports not only strings, but also abstract data types:

- Lists of strings
- Sets of strings (collections of non-repeating unsorted elements)
- Sorted sets of strings (collections of non-repeating elements ordered by a floating-point number called score)
- Hash tables where keys and values are strings

To implement our cache, we are using a hash table. The key that identifies the table is the name of the object type. Each entry in the table is identified by the object id and the value is the object itself serialed.


# API
The Applied solution has a AABC.Cache project, that contains all classes related to the management of a Redis database.

HashCacheRepository is a generic class that manages a hash set in redis.

It takes two type parameters:

- The type of the object that is being stored
This type must be serializable since it is being stored as string in redis.
The name of the type will be used as the key for the hash table. Therefore, if you want to store different hash tables for the same type, you must provide a different hash key name for the table (through the constructor), or else, you will be performing operations on the same hash table, with unpredictable results.

- The type of the field that identifies the object.
The key will be converted to a string by calling the ToString method.

HashCacheRepository needs a CacheClient to connect to the redis database. The CacheClient can outlive the HashCacheRepository. It is recommended that you store a single instance of the CacheClient, for instance, as a singleton.

HashCacheRepository is also responsible for setting the expiration time of the values being stored. It can be passed as a parameter through the constructor.

bool IsLoaded()
Returns true if the hash table exists, false otherwise.

bool Exists(TKey key)
Returns true if the key exists in the hash table, false otherwise.

TEntity Get(TKey key)
Returns the object with a key matching the value passed as parameter from the hash table. If there is no object with such key, it returns null.

IQueryable<TEntity> GetAll()
Returns a list of all objects in the hash table.

void Set(TEntity item)
Adds or replaces the object in the hash table.

public void SetAll(IEnumerable<TEntity> list)
Adds or replaces a list of objects in the hash table.

bool Remove(TKey key)
Removes the object with a key matching the value passed as parameter from the hash table. If such object is found, it returns true, otherwise false.

bool Remove(TEntity item)
Removes the object from the hash table. If such object is found, it returns true, otherwise false.

RemoveAll(IEnumerable<TEntity> items)
Removes all objects passed as parameters (if found) from the hash table.

bool RemoveAll()
Removes all objecsts from the hash table. The hash table is also removed from the database.



## Updating and Inserts
In the Web project, ProviderSearchService and PatientSearchService are responsible for returning the list of items. They both have a reference to the db context and also to a HashCacheRepository.

These services assume that the entries already exist in the db, and therefore doesn't have a concept of "new" entries: it will read the db and update/add as required.  Assumes that all entries are persisted to the db (even new ones) prior to being "updated" in the cache.  Thus, just use UpdateEntry/UpdateEntries for new entities as well as ones that were previously existing in the cache.

These services also offer a method to remove objects from the hash table.
