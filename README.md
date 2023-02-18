# ListNodeSerialize

Программа позволяет сериализовать и десериализовать двусвязаный список, в котором каждый элемент может дополнительно иметь ссылку на другой произвольный.

## Режимы работы
- **s - Serialize**  
Программно создаётся следующий тестовый список и записывается в файл "ListTest":  
![alt text](https://github.com/pusher71/ListNodeSerialize/blob/main/list_node_serialize/ListTestImage.png?raw=true)

- **d - Deserialize**  
Считывается ранее записанный список из файла "ListTest", и для каждого элемента списка в консоль выводятся его данные (Data) и данные произвольного элемента (Rand data):  
![alt text](https://github.com/pusher71/ListNodeSerialize/blob/main/list_node_serialize/ListTestDataImage.png?raw=true)

## Модификация тестовых данных
Сериализуемый тестовый список может быть изменён в классе **Program** в методе **SerializeTest**.
