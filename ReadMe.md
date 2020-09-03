# 模仿Prism的PubSubEvent实现了自己的WeakReferencePubSubEvent

改善了Prism的弱引用`PubSubEvent`会出现内存泄漏的问题。

# Prism的弱引用PubSubEvent出现内存泄漏的原因

Lambda在,Net Core中是一个委托。

.Net Core编译Lambda时，如果Lambda内部没有使用该Lambda所在的类的非静态成员或者方法时，编译器会为Lambda所在类生成一个内部类，该内部类持有一个自己类型的公共静态只读的字段，该字段会被赋值给Lambda的`Target`。

而Prism的`PubSubEvent`仅仅只是对Lambda的`Target`对象进行了`WeakReference`包装，但是由于.Net Core的编译器已经将该`Target`编译为了公共全局静态只读，所以该Target应该没有被析构的可能了。因此该订阅会一直存在，直到手动取消订阅。

# 解决办法

我自己实现的`WeakReferencePubSubEvent`会要求设置一个订阅者，订阅者与Lambda使用`ConditionalWeakTable<object, List<Action>`绑定在一起，订阅者没有被回收时，`List<Action>`也会被回收。
`ConditionalWeakTable<Key,Value>`是一个对Key进行弱引用的类似字典的数据结构，Key如果被GC回收，那么`ConditionalWeakTable<Key,Value>`不会继续引用Value。

